namespace Mkfeina.Domain.Panels
{
    public class PanelFactory
    {
        internal PanelFactory()
        {
        }

        public Panel CreatePanel(PanelConfig config)
        {
            if (!ConsoleSpaceManager.RegisterSpaceUsage(config.OriginLeft, config.OriginTop, config.Width, config.Hight))
                return null;
            else
                return new Panel(config);
        }
    }
}