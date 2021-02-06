﻿using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSE.Carrinho.API.Data;
using NSE.Carrinho.API.Models;
using NSE.WebApi.Core.Usuario;
using System.Threading.Tasks;

namespace NSE.Carrinho.API.Services.gRPC
{
    [Authorize]
    public class CarrinhoGrpcService : CarrinhoCompras.CarrinhoComprasBase
    {
        private readonly IAspNetUser _user;
        private readonly ILogger<CarrinhoGrpcService> _logger;
        private readonly CarrinhoContext _context;

        public CarrinhoGrpcService
        (
            IAspNetUser user,
            ILogger<CarrinhoGrpcService> logger,
            CarrinhoContext context
        )
        {
            _user = user;
            _logger = logger;
            _context = context;
        }

        public override async Task<CarrinhoClienteResponse> ObterCarrinho(ObterCarrinhoRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Chamando ObterCarrinho");

            var carrinho = await ObterCarrinhoCliente() ?? new CarrinhoCliente();

            return ParaCarrinhoResponse(carrinho);
        }

        private async Task<CarrinhoCliente> ObterCarrinhoCliente()
        {
            return await _context.CarrinhoCliente
                .Include(x => x.Itens)
                .FirstOrDefaultAsync(x => x.ClienteId == _user.ObterUserId());
        }

        private static CarrinhoClienteResponse ParaCarrinhoResponse(CarrinhoCliente carrinho)
        {
            var carrinhoProto =  new CarrinhoClienteResponse
            {
                Id = carrinho.Id.ToString(),
                Clienteid = carrinho.ClienteId.ToString(),
                Valortotal = (double)carrinho.ValorTotal,
                Desconto = (double)carrinho.Desconto,
                Voucherutilizado = carrinho.VoucherUtilizado
            };

            if(carrinho.Voucher != null)
            {
                carrinhoProto.Voucher = new VoucherResponse
                {
                    Codigo = carrinho.Voucher.Codigo,
                    Percentual = (double?)carrinho.Voucher.Percentual ?? 0,
                    Valordesconto =(double?)carrinho.Voucher.ValorDesconto ?? 0,
                    Tipodesconto = (int)carrinho.Voucher.TipoDesconto,

                };
            }

            foreach (var item in carrinho.Itens)
            {
                carrinhoProto.Itens.Add(new CarrinhoItemResponse
                {
                    Id = item.Id.ToString(),
                    Imagem = item.Imagem,
                    Nome = item.Nome,
                    Produtoid = item.ProdutoId.ToString(),
                    Quantidade = item.Quantidade,
                    Valor = (double)item.Valor
                });
            }

            return carrinhoProto;
        }
    }
}
