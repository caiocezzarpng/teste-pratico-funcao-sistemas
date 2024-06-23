using FI.AtividadeEntrevista.BLL;
using FI.AtividadeEntrevista.DML;
using FI.WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using WebAtividadeEntrevista.Models;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model, string beneficiariosJson)
        {
            try
            {
                BoCliente bo = new BoCliente();
                BoBeneficiario boBeneficiario = new BoBeneficiario();

                if (!this.ModelState.IsValid)
                {
                    List<string> erros = (from item in ModelState.Values
                                          from error in item.Errors
                                          select error.ErrorMessage).ToList();

                    Response.StatusCode = 400;
                    return Json(string.Join(Environment.NewLine, erros));
                }
                else
                {
                    List<BeneficiarioModel> beneficiarioModels = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BeneficiarioModel>>(beneficiariosJson);

                    if (bo.VerificarExistencia(model.CPF, model.Id))
                    {
                        Response.StatusCode = 409;
                        return Json("CPF já cadastrado");
                    }

                    model.Id = bo.Incluir(new Cliente()
                    {
                        CEP = model.CEP,
                        Cidade = model.Cidade,
                        Email = model.Email,
                        Estado = model.Estado,
                        Logradouro = model.Logradouro,
                        Nacionalidade = model.Nacionalidade,
                        Nome = model.Nome,
                        Sobrenome = model.Sobrenome,
                        Telefone = model.Telefone,
                        CPF = model.CPF
                    });

                    if (beneficiarioModels != null)
                    {
                        Regex regex = new Regex("[^0-9]");

                        foreach (BeneficiarioModel beneficiario in beneficiarioModels)
                        {
                            beneficiario.CPF = regex.Replace(beneficiario.CPF, string.Empty);

                            boBeneficiario.Incluir(new Beneficiario()
                            {
                                CPF = beneficiario.CPF,
                                Nome = beneficiario.Nome,
                                ClienteId = model.Id
                            });
                        }
                    }

                    return Json("Cadastro efetuado com sucesso");
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model, string beneficiariosJson)
        {
            BoCliente boCliente = new BoCliente();
            BoBeneficiario boBeneficiario = new BoBeneficiario();
       
            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if (boCliente.VerificarExistencia(model.CPF, model.Id))
                {
                    Response.StatusCode = 409;
                    return Json("CPF já cadastrado");
                }

                Regex regex = new Regex("[^0-9]");
                List<BeneficiarioModel> beneficiarioModels = new List<BeneficiarioModel>();
                beneficiarioModels = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BeneficiarioModel>>(beneficiariosJson);

                foreach (BeneficiarioModel beneficiario in beneficiarioModels)
                {
                    string action = beneficiario.Action ?? "None";

                    if (action.Equals("Register", StringComparison.InvariantCultureIgnoreCase))
                    {
                        boBeneficiario.Incluir(new Beneficiario()
                        {
                            CPF = regex.Replace(beneficiario.CPF, string.Empty),
                            Nome = beneficiario.Nome,
                            ClienteId = model.Id
                        });
                    }
                    else if (action.Equals("Remove", StringComparison.InvariantCultureIgnoreCase))
                    {
                        boBeneficiario.Excluir(beneficiario.Id);
                    }
                    else if (action.Equals("Update", StringComparison.InvariantCultureIgnoreCase))
                    {
                        boBeneficiario.Alterar(new Beneficiario()
                        {
                            Id = beneficiario.Id,
                            CPF = regex.Replace(beneficiario.CPF, string.Empty),
                            Nome = beneficiario.Nome,
                            ClienteId = model.Id
                        });
                    }
                }

                boCliente.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF
                });
                               
                return Json("Cadastro alterado com sucesso");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    CPF = cliente.CPF
                };

                List<Beneficiario> beneficiarios = new BoBeneficiario().Pesquisa(id);
                
                foreach (Beneficiario beneficiario in beneficiarios)
                {
                    model.Beneficiarios.Add(new BeneficiarioModel()
                    {
                        Id = beneficiario.Id,
                        CPF = CpfMask(beneficiario.CPF),
                        Nome = beneficiario.Nome,
                        ClienteId = beneficiario.ClienteId
                    });
                }
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Excluir(long id)
        {
            try
            {
                new BoCliente().Excluir(id);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        private string CpfMask(string cpf)
        {
            return Regex.Replace(cpf, @"(\d{3})(\d{3})(\d{3})(\d{2})", "$1.$2.$3-$4");
        }
    }
}