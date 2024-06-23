using FI.AtividadeEntrevista.DML;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FI.AtividadeEntrevista.DAL
{
    /// <summary>
    /// Classe de acesso a dados do Beneficiario
    /// </summary>
    internal class DaoBeneficiario : AcessoDados
    {
        internal void Incluir(DML.Beneficiario beneficiario)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>
            {
                new System.Data.SqlClient.SqlParameter("CPF", beneficiario.CPF),
                new System.Data.SqlClient.SqlParameter("Nome", beneficiario.Nome),
                new System.Data.SqlClient.SqlParameter("ClienteID", beneficiario.ClienteId)
            };

            DataSet ds = base.Consultar("FI_SP_IncBeneficiario", parametros);
        }

        internal void Alterar(DML.Beneficiario beneficiario)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>
            {
                new System.Data.SqlClient.SqlParameter("CPF", beneficiario.CPF),
                new System.Data.SqlClient.SqlParameter("Nome", beneficiario.Nome),
                new System.Data.SqlClient.SqlParameter("ClienteID", beneficiario.ClienteId),
                new System.Data.SqlClient.SqlParameter("ID", beneficiario.Id)
            };

            base.Executar("FI_SP_AltBenef", parametros);
        }


        /// <summary>
        /// Excluir Beneficiario
        /// </summary>
        /// <param name="beneficiario">Objeto beneficiario</param>
        internal void Excluir(long Id)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>
            {
                new System.Data.SqlClient.SqlParameter("Id", Id)
            };

            base.Executar("FI_SP_DelBeneficiario", parametros);
        }

        internal bool VerificarExistencia(string cpf, long id)
        {
            throw new NotImplementedException();
        }

        internal DML.Beneficiario Consultar(long Id)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>
            {
                new System.Data.SqlClient.SqlParameter("Id", Id)
            };

            DataSet ds = base.Consultar("FI_SP_ConsBeneficiario", parametros);
            List<DML.Beneficiario> beneficiarios = Converter(ds);

            return beneficiarios.FirstOrDefault();
        }

        internal List<Beneficiario> Pesquisa(long id)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("ClienteID", id));

            DataSet ds = base.Consultar("FI_SP_PesqBeneficiario", parametros);
            List<DML.Beneficiario> beneficiarios = Converter(ds);

            return beneficiarios;
        }

        private List<DML.Beneficiario> Converter(DataSet ds)
        {
            List<DML.Beneficiario> lista = new List<DML.Beneficiario>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DML.Beneficiario beneficiario = new DML.Beneficiario();
                    beneficiario.Id = row.Field<long>("ID");
                    beneficiario.ClienteId = row.Field<long>("IDCLIENTE");
                    beneficiario.Nome = row.Field<string>("NOME");
                    beneficiario.CPF = row.Field<string>("CPF");
                    lista.Add(beneficiario);
                }
            }

            return lista;
        }
    }
}
