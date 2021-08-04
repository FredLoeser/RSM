using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperConsultaXML.Models
{
	public class Pedido
	{
        public Pedido()
        {
            Produtos = new HashSet<Produto>();
        }

        public string Cliente { get; set; }
        public int NumeroPedido { get; set; }
        public DateTime Data { get; set; }
        public decimal Total { get; set; }

        public ICollection<Produto> Produtos { get; set; }

        public class Produto
        {
            public string Codigo { get; set; }
            public string Nome { get; set; }
            public int Quantidade { get; set; }
            public decimal Valor { get; set; }
        }
    }
}
