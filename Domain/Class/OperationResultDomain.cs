
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;

namespace Domain.Class
{
    public class OperationResultDomain
    {
        public decimal BitcoinPrice { get; set; }
        public bool Executed { get; set; }
        public decimal UsdtBalance { get; set; }
        public DateTime OperationDate { get; set; }
        public int Decision { get; set; }
        public decimal MarketRsi { get; set; }
        public int KlineInterval { get; set; }
        public int BuyRsi { get; set; }
        public int SellRsi { get; set; }
        public int TakeProfit { get; set; }
        public int StopLoss { get; set; }
    }
}
