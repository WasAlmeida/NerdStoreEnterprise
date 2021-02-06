﻿using Dapper;
using Microsoft.EntityFrameworkCore;
using NSE.Catalogo.API.Models;
using NSE.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.Catalogo.API.Data.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly CatologoContext _context;

        public ProdutoRepository(CatologoContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Produto> ObterPorId(Guid id)
        {
            return await _context.Produtos.FindAsync(id);
        }

        public async Task<PagedResult<Produto>> ObterTodos(int pageSize, int pageIndex, string query = null)
        {
            var sql = @$"SELECT * FROM Produtos 
                      WHERE (@Nome IS NULL OR Nome LIKE '%' + @Nome + '%') 
                      ORDER BY [Nome] 
                      OFFSET {pageSize * (pageIndex - 1)} ROWS 
                      FETCH NEXT {pageSize} ROWS ONLY 
                      SELECT COUNT(Id) FROM Produtos 
                      WHERE (@Nome IS NULL OR Nome LIKE '%' + @Nome + '%')";

            var multi = await _context.Database.GetDbConnection()
                .QueryMultipleAsync(sql, new { Nome = query });

            var produtos = multi.Read<Produto>();
            var total = multi.Read<int>().FirstOrDefault();

            return new PagedResult<Produto>()
            {
                List = produtos,
                TotalResults = total,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Query = query
            };
        }

        public async Task Adicionar(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
        }

        public void Atualizar(Produto produto)
        {
            _context.Produtos.Update(produto);
        }

        public async Task<List<Produto>> ObterProdutosPorId(string ids)
        {
            var idsGuid = ids.Split(",")
                .Select(id => (Ok: Guid.TryParse(id, out var x), Value: x));

            if (!idsGuid.All(nid => nid.Ok)) return new List<Produto>();

            var idsValue = idsGuid.Select(x => x.Value);

            return await _context.Produtos.AsNoTracking()
                .Where(p => idsValue.Contains(p.Id) && p.Ativo).ToListAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
