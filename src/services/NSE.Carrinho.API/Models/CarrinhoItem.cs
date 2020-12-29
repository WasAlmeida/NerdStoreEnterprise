﻿using FluentValidation;
using System;

namespace NSE.Carrinho.API.Models
{
    public class CarrinhoItem
    {
        public CarrinhoItem()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public Guid ProdutoId { get; set; }
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
        public string Imagem { get; set; }
        public Guid CarrinhoId { get; set; }
        public CarrinhoCliente CarrinhoCliente { get; set; }

        internal void AssociarCarrinho(Guid carrinhoId)
        {
            CarrinhoId = carrinhoId;
        }

        internal decimal CalcularValor()
        {
            return Quantidade * Valor;
        }

        internal void AdicionarUnidades(int unidades)
        {
            Quantidade += unidades;
        }

        internal void AtualizarUnidades(int quantidade)
        {
            Quantidade = quantidade;
        }

        internal bool EhValido()
        {
            return new ItemCarrinhoValidation().Validate(this).IsValid;
        }
    }

    public class ItemCarrinhoValidation : AbstractValidator<CarrinhoItem>
    {
        public ItemCarrinhoValidation()
        {
            RuleFor(x => x.ProdutoId)
                .NotEqual(Guid.Empty)
                .WithMessage("Id produto inválido.");

            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("O nome do produto não foi informado.");

            RuleFor(x => x.Quantidade)
                .GreaterThan(0)
                .WithMessage(item =>  $"A quantidade miníma para o item {item.Nome} é 1.");

            RuleFor(x => x.Quantidade)
                .LessThan(CarrinhoCliente.QuantidadeMaximaItem)
                .WithMessage(item =>  $"A quantidade máxima do item {item.Nome} é {CarrinhoCliente.QuantidadeMaximaItem}.");

            RuleFor(x => x.Valor)
                .GreaterThan(0)
                .WithMessage(item => $"O valor do {item.Nome} precisa ser maior do que 0.");
        }
    }
}