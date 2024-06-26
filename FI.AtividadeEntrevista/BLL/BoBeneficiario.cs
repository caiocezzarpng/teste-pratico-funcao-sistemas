﻿using System.Collections.Generic;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoBeneficiario
    {
        private readonly DAL.DaoBeneficiario _beneficiarioDAO;

        public BoBeneficiario()
        {
            _beneficiarioDAO = new DAL.DaoBeneficiario(); 
        }
        /// <summary>
        /// Inclui um novo beneficiario
        /// </summary>
        /// <param name="beneficiario">Objeto de beneficiario</param>
        public void Incluir(DML.Beneficiario beneficiario)
        {
            _beneficiarioDAO.Incluir(beneficiario);
        }

        /// <summary>
        /// Altera um beneficiario
        /// </summary>
        /// <param name="beneficiario">Objeto de beneficiario</param>
        public void Alterar(DML.Beneficiario beneficiario)
        {
            _beneficiarioDAO.Alterar(beneficiario);
        }

        /// <summary>
        /// Excluir o beneficiario pelo id
        /// </summary>
        /// <param name="id">id do beneficiario</param>
        /// <returns></returns>
        public void Excluir(long id)
        {
            _beneficiarioDAO.Excluir(id);
        }

        /// <summary>
        /// verifica se ja existe um beneficiario com o mesmo cpf para o mesmo cliente
        /// </summary>
        /// <param name="id">id do beneficiario</param>
        /// <returns></returns>
        public bool VerificarExistencia(string CPF, long id)
        {
            return _beneficiarioDAO.VerificarExistencia(CPF, id);
        }

        /// <summary>
        /// pesquisa os beneficiarios de um cliente
        /// </summary>
        /// <param name="id">id do cliente</param>
        /// <returns></returns>
        public List<DML.Beneficiario> Pesquisa(long clienteId)
        {
            return _beneficiarioDAO.Pesquisa(clienteId);
        }
    }
}