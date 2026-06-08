#!/usr/bin/env bash
#  TerraNova — Provisionamento de Infraestrutura Azure
#  Projeto: Monitoramento Agrícola com NASA Power + SatVeg
#  Banco  : Oracle XE 21c   |   App: .NET (TerraNova API)

set -e

# Variáveis do Ambiente
RG="rg-terranova"
LOCATION="eastus"
VNET="vnet-terranova"
SUBNET="subnet-terranova"
NSG="nsg-terranova"
VM="vm-terranova"
ADMIN="terranova-adm"

# Grupo de Recursos
echo "[1/6] Criando Grupo de Recursos..."
az group create \
  --name "$RG" \
  --location "$LOCATION"

# VNet e Subnet
echo "[2/6] Criando VNet e Subnet..."
az network vnet create \
  --resource-group "$RG" \
  --location "$LOCATION" \
  --name "$VNET" \
  --address-prefixes 10.20.0.0/16 \
  --subnet-name "$SUBNET" \
  --subnet-prefixes 10.20.1.0/24

# Network Security Group
echo "[3/6] Criando Network Security Group..."
az network nsg create \
  --resource-group "$RG" \
  --location "$LOCATION" \
  --name "$NSG"

# Máquina Virtual Ubuntu 22.04 LTS
echo "[4/6] Criando Máquina Virtual Linux..."
az vm create \
  --resource-group "$RG" \
  --name "$VM" \
  --image Ubuntu2204 \
  --size Standard_B2s \
  --admin-username "$ADMIN" \
  --generate-ssh-keys \
  --output json \
  --verbose \
  --vnet-name "$VNET" \
  --subnet "$SUBNET" \
  --nsg "$NSG"

# Liberar Portas
echo "[5/6] Liberando portas necessárias..."
az vm open-port --resource-group "$RG" --name "$VM" --port 22   --priority 1000
az vm open-port --resource-group "$RG" --name "$VM" --port 8080 --priority 1010
az vm open-port --resource-group "$RG" --name "$VM" --port 1521 --priority 1020

# Instalar Docker e Dependências na VM
echo "[6/6] Instalando Docker e dependências na VM..."
az vm run-command invoke \
  --resource-group "$RG" \
  --name "$VM" \
  --command-id RunShellScript \
  --scripts "
    # Atualizar pacotes
    sudo apt-get update -y

    # Instalar dependências base
    sudo apt-get install -y \
      ca-certificates \
      curl \
      gnupg \
      git \
      jq \
      nano \
      unzip \
      wget

    # Adicionar repositório oficial do Docker
    sudo install -m 0755 -d /etc/apt/keyrings
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg | \
      sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
    sudo chmod a+r /etc/apt/keyrings/docker.gpg

    echo \
      \"deb [arch=\$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] \
      https://download.docker.com/linux/ubuntu \
      \$(. /etc/os-release && echo \"\$VERSION_CODENAME\") stable\" | \
      sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

    # Instalar Docker Engine + Compose Plugin
    sudo apt-get update -y
    sudo apt-get install -y \
      docker-ce \
      docker-ce-cli \
      containerd.io \
      docker-buildx-plugin \
      docker-compose-plugin

    # Iniciar e habilitar no boot
    sudo systemctl start docker
    sudo systemctl enable docker

    # Adicionar admin ao grupo docker (sem precisar de sudo)
    sudo usermod -aG docker $ADMIN

    # Verificar instalação
    docker --version
    docker compose version

    echo '============================================'
    echo ' Docker instalado com sucesso na VM!'
    echo '============================================'
  "

# Exibir resumo final
PUBLIC_IP=$(az vm show \
  --resource-group "$RG" \
  --name "$VM" \
  --show-details \
  --query publicIps \
  --output tsv)

echo "======================================================"
echo "  Provisionamento concluído com sucesso!"
echo "  IP Público da VM  : $PUBLIC_IP"
echo "  Acesse via SSH    : ssh $ADMIN@$PUBLIC_IP"
echo "  API (após deploy) : http://$PUBLIC_IP:8080"
echo "  Swagger           : http://$PUBLIC_IP:8080"
echo "  Oracle (externo)  : $PUBLIC_IP:1521  (SID: XE / PDB: XEPDB1)"
echo "======================================================"