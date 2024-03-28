using QuickFix.Fields;

public class Order
{
    public string OrderID { get; }    
    public string ClOrdID { get; }
    public string Symbol { get; set; }
    public bool IsBuy { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    // Construtor que aceita um objeto NewOrderSingle
    public Order(QuickFix.FIX44.NewOrderSingle newOrderSingle)
    {
        ClOrdID = newOrderSingle.ClOrdID.Obj;
        Symbol = newOrderSingle.Symbol.Obj;
        IsBuy = newOrderSingle.Side.Obj == Side.BUY;
        Quantity = (int)newOrderSingle.OrderQty.Obj;
        Price = (decimal)newOrderSingle.Price.Obj;
    }
}
