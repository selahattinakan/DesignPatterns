namespace WebApp.ChainOfResponsibilty.ChainOfResponsibility
{
    public interface IProcessHandler
    {
        IProcessHandler SetNext(IProcessHandler processHandler);

        Object handle(Object o);


    }
}
