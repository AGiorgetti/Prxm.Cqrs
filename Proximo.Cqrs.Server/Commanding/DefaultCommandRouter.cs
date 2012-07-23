using System.Reflection;
using Proximo.Cqrs.Core.Commanding;
using Proximo.Cqrs.Core.Support;

namespace Proximo.Cqrs.Server.Commanding
{
    /// <summary>
    /// this is the default router for commands, it simply execute in process the
    /// command given an handler.
    /// </summary>
    public class DefaultCommandRouter : ICommandRouter
    {
        private ICommandHandlerCatalog _commandHandlerCatalog;
        
        private ILogger _logger;

        public DefaultCommandRouter(ICommandHandlerCatalog commandHandlerCatalog, ILogger logger)
        {
            _commandHandlerCatalog = commandHandlerCatalog;
            _logger = logger;
        }

        public void RouteToHandler(ICommand command)
        {
            //optype set in logger context information about the logical operation that the system is executing
            //is used to group log messages togheter and to correlate child log to a logical operation.
            _logger.SetInThreadContext("op_type", "command " + command.Id + " " + command.GetType().FullName);
            
            _logger.Info("[queue] processing command " + command.ToString());

            var commandType = command.GetType();
            
            //get the executor function from the catalog, and then simply execute the command.
            var commandinvoker = _commandHandlerCatalog.GetExecutorFor(commandType);
            commandinvoker.Invoke(command);

            //this is the old code, still working.
            //var commandHandlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
            //var consumer = _handlerFactory.CreateHandler(commandHandlerType);

            //// we are assuming sync execution and we object tracking by the container
            //// todo: change the lifestyle to a truly transient one ?
            //MethodInfo mi = commandHandlerType.GetMethod("Handle", new[] { commandType });
            //mi.Invoke(consumer, new object[] { command });

            _logger.Info("[queue] command handled " + command.ToString());

            _logger.RemoveFromThreadContext("op_type");
        }
    }
}