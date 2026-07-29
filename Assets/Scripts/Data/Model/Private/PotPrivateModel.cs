namespace Data
{
    public class PotPrivateModel : PrivateModel.Collection<PotPrivateScheme>
    {
        protected override PotPrivateScheme CreateSchemeById(string id) => 
            !string.IsNullOrEmpty(id) ? new PotPrivateScheme(id) : null;
    }
}
