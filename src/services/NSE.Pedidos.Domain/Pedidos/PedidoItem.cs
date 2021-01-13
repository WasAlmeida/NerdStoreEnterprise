﻿using NSE.Core.DomainObjects;
using System;

namespace NSE.Pedidos.Domain.Pedidos
{
    public class PedidoItem : Entity
    {
        public Guid PedidoId { get; private set; }
        public Guid ProdudoId { get; private set; }
        public string ProdutoNome { get; private set; }
        public int Quantidade { get; private set; }
        public decimal ValorUnitario { get; private set; }
        public string ProdutoImagem { get; set; }
        //EF. Rel
        public Pedido Pedido { get; set; }

        public PedidoItem(Guid produtoId, string produtoNome, int quantidade, 
            decimal valorUnitario, string produtoImagem = null)
        {
            ProdudoId = produtoId;
            ProdutoNome = produtoNome;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
            ProdutoImagem = produtoImagem;
        }
        protected PedidoItem() { }

        internal decimal CalcularValor()
        {
            return ValorUnitario * Quantidade;
        }
    }
}