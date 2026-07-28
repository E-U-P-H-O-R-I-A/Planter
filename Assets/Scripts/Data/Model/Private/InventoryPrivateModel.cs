namespace Data
{
    public class InventoryPrivateModel : PrivateModel.Collection<ItemPrivateScheme>
    {
        protected override ItemPrivateScheme CreateSchemeById(string id)
        {
            return new ItemPrivateScheme(id);
        }
    }
}