using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SuperConsultaXML.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace SuperConsultaXML.Controllers
{
	public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; 
        private IHostingEnvironment _environment;

        public HomeController(ILogger<HomeController> logger, IHostingEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        [HttpGet]
        [Route("{pagina?}")]
        public IActionResult Index(int? pagina)
        {
            var pedidosVendas = CarregaXml();

            if (pagina == null)
                pagina = 1;

            if (pagina != null && pagina < 1)
                pagina = 1;

            if (pagina != null && pagina > 4)
                pagina = 4;

            var meuModelBonitao = new IndexViewModel
            {
                Pedido = pedidosVendas.Skip(pagina.Value - 1).FirstOrDefault(),
                Pagina = pagina.Value
            };

            return View(meuModelBonitao);
        }

        [HttpPost]
        public IActionResult Index(dynamic param)
        {
            var pedidosVendas = CarregaXml();

            param = "Index";
            return View(pedidosVendas);
        }

        [HttpPost]
        public IActionResult Teste(dynamic param)
        {
            param = "Teste";
            return View("Index", (object)param);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private ICollection<Pedido> CarregaXml()
        {
            string wwwPath = _environment.WebRootPath;
            var pedidosVendas = new HashSet<Pedido>();

            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(wwwPath + @"\xml\pedidos (002).xml");
            
            var xmlPedidos = doc.GetElementsByTagName("Pedido");
            foreach (XmlElement xmlPedido in xmlPedidos)
			{
                if (xmlPedido.SelectSingleNode("Cliente") != null)
                {
                    var pedido = new Pedido();

                    pedido.Cliente = xmlPedido.SelectSingleNode("Cliente") != null ? Convert.ToString(xmlPedido.SelectSingleNode("Cliente").InnerText) : "";
                    pedido.NumeroPedido = xmlPedido.SelectSingleNode("Pedido") != null ? Convert.ToInt32(xmlPedido.SelectSingleNode("Pedido").InnerText) : 0;
                    pedido.Data = xmlPedido.SelectSingleNode("Data") != null ? Convert.ToDateTime(xmlPedido.SelectSingleNode("Data").InnerText) : DateTime.MinValue;
                    pedido.Total = xmlPedido.SelectSingleNode("Total") != null ? Convert.ToDecimal(xmlPedido.SelectSingleNode("Total").InnerText) : 0;

                    pedidosVendas.Add(pedido);

                    var xmlProdutos = xmlPedido.GetElementsByTagName("Produto");
                    foreach (XmlElement xmlProduto in xmlProdutos) {

                        var produto = new Pedido.Produto();

                        produto.Codigo = xmlProduto.SelectSingleNode("Codigo") != null ? Convert.ToString(xmlProduto.SelectSingleNode("Codigo").InnerText) : "";
                        produto.Nome = xmlProduto.SelectSingleNode("Nome") != null ? Convert.ToString(xmlProduto.SelectSingleNode("Nome").InnerText) : "";
                        produto.Quantidade = xmlProduto.SelectSingleNode("Quantidade") != null ? Convert.ToInt32(xmlProduto.SelectSingleNode("Quantidade").InnerText) : 0;
                        produto.Valor = xmlProduto.SelectSingleNode("Valor") != null ? Convert.ToDecimal(xmlProduto.SelectSingleNode("Valor").InnerText) : 0;

                        pedido.Produtos.Add(produto);
                    }
                }
                else
				{
                    continue;
				}
			}

            var json = JsonConvert.SerializeObject(pedidosVendas);

            return pedidosVendas;
        }
    }
}
