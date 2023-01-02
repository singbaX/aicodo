namespace AiCodo.WpfApp
{
    public class MainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterViews(typeof(MainModule).Assembly);

            base.Load(builder);
        }
    }
}
