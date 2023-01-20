using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treinamento_PluginTelefone
{
    public class WFValidaExistenciaEmailFoneAluno : CodeActivity
    {
        #region Parametros
        // recebe o usuário do contexto
        [Input("Usuario")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> usuarioEntrada { get; set; }

        // recebe o contexto
        [Input("Aluno")]
        [ReferenceTarget("new_aluno")]
        public InArgument<EntityReference> RegistroContexto { get; set; }

        [Output("Saida")]
        public OutArgument<string> saida { get; set; }
        #endregion
        protected override void Execute(CodeActivityContext executionContext)
        {
            //Create the context            
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService trace = executionContext.GetExtension<ITracingService>();

            // informação para o Log de Rastreamento de Plugin
            trace.Trace("new_aluno: " + context.PrimaryEntityId);

            // declaro variável com o Guid da entidade primária em uso
            Guid guidAluno = context.PrimaryEntityId;

            // informação para o Log de Rastreamento de Plugin
            trace.Trace("guidAluno: " + guidAluno);

            // Variável do tipo Entity vazia
            Entity entidadeContexto = null;

            // Herda a entidade do contexto
            entidadeContexto = (Entity)context.InputParameters["Target"];

            #region Trace
            trace.Trace("Entidade do Contexto recebida: " + entidadeContexto.LogicalName);
            trace.Trace("Entidade do Contexto - Atributos: " + entidadeContexto.Attributes.Count);
            if (entidadeContexto.Attributes.Contains("new_email"))
            {
                trace.Trace("Email do Contexto: " + entidadeContexto["new_email"]);
            }
            if (entidadeContexto.Attributes.Contains("new_telefone"))
            {
                trace.Trace("Fone do Contexto: " + entidadeContexto["new_telefone"]);
            }
            #endregion

            // valida Email
            if (entidadeContexto.Attributes.Contains("new_email"))
            {
                var emailAluno = entidadeContexto["new_email"];

                String fetchEmail = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>";
                fetchEmail += "<entity name='new_aluno'>";
                fetchEmail += "<attribute name='new_alunoid' />";
                fetchEmail += "<attribute name='new_name' />";
                fetchEmail += "<attribute name='createdon' />";
                fetchEmail += "<order attribute='new_name' descending='false' />";
                fetchEmail += "<filter type='and'>";
                fetchEmail += "<condition attribute='new_email' operator='eq' value='" + emailAluno.ToString().Trim() + "' />";
                fetchEmail += "<condition attribute='new_alunoid' value='" + guidAluno + "' uitype='new_aluno' operator='ne'/>";
                fetchEmail += "</filter>";
                fetchEmail += "</entity>";
                fetchEmail += "</fetch>";
                var entAlunoEmail = service.RetrieveMultiple(new FetchExpression(fetchEmail));

                trace.Trace("entAlunoEmail: " + entAlunoEmail.Entities.Count);

                if (entAlunoEmail.Entities.Count > 0)
                {
                    throw new InvalidPluginExecutionException("Email já existente na base de Alunos!");
                }
            }

            // valida telefone
            if (entidadeContexto.Attributes.Contains("new_telefone"))
            {
                var foneAluno = entidadeContexto["new_telefone"];

                String fetchFone = "<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>";
                fetchFone += "<entity name='new_aluno'>";
                fetchFone += "<attribute name='new_alunoid'/>";
                fetchFone += "<attribute name='new_name'/>";
                fetchFone += "<attribute name='createdon'/>";
                fetchFone += "<order descending='false' attribute='new_name'/>";
                fetchFone += "<filter type='and'>";
                fetchFone += "<condition attribute='new_telefone' value='" + foneAluno.ToString().Trim() + "' operator='eq'/>";
                fetchFone += "<condition attribute='new_alunoid' value='" + guidAluno + "' uitype='new_aluno' operator='ne'/>";
                fetchFone += "</filter>";
                fetchFone += "</entity>";
                fetchFone += "</fetch>";
                var entAlunoFone = service.RetrieveMultiple(new FetchExpression(fetchFone));
                if (entAlunoFone.Entities.Count > 0)
                {
                    throw new InvalidPluginExecutionException("Telefone já existente na base de Alunos!");
                }
            }

        }
    }
}
