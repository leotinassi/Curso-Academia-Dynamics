using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TreinamentoII
{
    public class PluginAccountPreValidationSync : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            #region Contexto
            // Variável contendo o contexto da execução
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Variável contendo o Service Factory da Organização
            IOrganizationServiceFactory serviceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // Variável contendo o Service Admin que estabele os serviços de conexão com o Dataverse
            IOrganizationService serviceAdmin = serviceFactory.CreateOrganizationService(null);

            // Variável do Trace que armazena informações de LOG
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            #endregion

            // Variável do tipo Entity vazia
            Entity entidadeContexto = null;

            if (context.InputParameters.Contains("Target")) // Verifica se contém dados para o destino
            {
                entidadeContexto = (Entity)context.InputParameters["Target"]; // atribui o contexto da entidade para a variável

                #region Trace
                trace.Trace("Entidade do Contexto recebida: " + entidadeContexto.LogicalName);
                trace.Trace("Entidade do Contexto - Atributos: " + entidadeContexto.Attributes.Count); // armazena informações de LOG                                
                #endregion

                if (entidadeContexto == null) //verifica se a entidade do contexto está vazia
                {
                    return; // caso verdadeira retorna sem nada executar
                }

                if (!entidadeContexto.Contains("telephone1") && entidadeContexto.Id == Guid.Empty) // verifica se o atributo telephone1 não está presente no contexto
                {
                    throw new InvalidPluginExecutionException("Campo Telefone principal é obrigatório!"); // exibe Exception de Erro                 
                }
                else
                {
                    if (entidadeContexto.Contains("telephone1"))
                    {
                        trace.Trace("Valor do campo: " + entidadeContexto["telephone1"].ToString());
                        if (entidadeContexto["telephone1"].ToString() == String.Empty || entidadeContexto["telephone1"].ToString() == null
                                || entidadeContexto["telephone1"].ToString() == "")
                        {
                            throw new InvalidPluginExecutionException("Campo Telefone principal é obrigatório!"); // exibe Exception de Erro
                        }
                    }
                }

                // valida se o CNPJ já existe
                if (entidadeContexto.Contains("new_cnpj"))
                {
                    trace.Trace("CNPJ: " + entidadeContexto["new_cnpj"].ToString());
                    String cnpjValidar = entidadeContexto["new_cnpj"].ToString();

                    String fetchAccount = "<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>";
                    fetchAccount += "<entity name='account'>";
                    fetchAccount += "<attribute name='name'/>";
                    fetchAccount += "<attribute name='accountid'/>";
                    fetchAccount += "<order descending='false' attribute='name'/>";
                    fetchAccount += "<filter type='and'>";
                    fetchAccount += "<condition attribute='new_cnpj' value='" + cnpjValidar.Trim() + "' operator='eq'/>";
                    fetchAccount += "</filter>";
                    fetchAccount += "</entity>";
                    fetchAccount += "</fetch>";
                    var entAccountCNPJ = serviceAdmin.RetrieveMultiple(new FetchExpression(fetchAccount));

                    trace.Trace("entAccountCNPJ: " + entAccountCNPJ.Entities.Count);

                    if (entAccountCNPJ.Entities.Count > 0)
                    {
                        throw new InvalidPluginExecutionException("CNPJ já existente para o cliente: " + entAccountCNPJ.Entities[0].Attributes["name"].ToString());
                    }
                }
                else if (entidadeContexto.Contains("new_cpf"))
                {
                    trace.Trace("CNPJ: " + entidadeContexto["new_cpf"].ToString());
                    String cpfValidar = entidadeContexto["new_cpf"].ToString();

                    String fetchAccount = "<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>";
                    fetchAccount += "<entity name='account'>";
                    fetchAccount += "<attribute name='name'/>";
                    fetchAccount += "<attribute name='accountid'/>";
                    fetchAccount += "<order descending='false' attribute='name'/>";
                    fetchAccount += "<filter type='and'>";
                    fetchAccount += "<condition attribute='new_cpf' value='" + cpfValidar.Trim() + "' operator='eq'/>";
                    fetchAccount += "</filter>";
                    fetchAccount += "</entity>";
                    fetchAccount += "</fetch>";
                    var entAccountCPF = serviceAdmin.RetrieveMultiple(new FetchExpression(fetchAccount));

                    trace.Trace("entAccountCNPJ: " + entAccountCPF.Entities.Count);

                    if (entAccountCPF.Entities.Count > 0)
                    {
                        throw new InvalidPluginExecutionException("CPF já existente para o cliente: " + entAccountCPF.Entities[0].Attributes["name"].ToString());
                    }
                }
            }
        }
    }
}