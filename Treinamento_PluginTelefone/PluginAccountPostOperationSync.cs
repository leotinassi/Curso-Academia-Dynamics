using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Treinamento_PluginTelefone
{
    public class PluginAccountPostOperationSync : IPlugin 
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
            entidadeContexto = (Entity)context.InputParameters["Target"];

            // Variável para nova entidade Contato vazia
            var Contact = new Entity("contact");

            // atribuição dos atributos para novo registro da entidade Contato
            Contact["firstname"] = "Contato Assinc vinculado a Conta";
            Contact["lastname"] = entidadeContexto["name"];
            Contact["parentcustomerid"] = new EntityReference("account", context.PrimaryEntityId);
            Contact["ownerid"] = new EntityReference("systemuser", context.UserId);

            trace.Trace("firstname: " + Contact["firstname"]);

            serviceAdmin.Create(Contact); // executa o método Create para entidade Contato




        }
    }
}
