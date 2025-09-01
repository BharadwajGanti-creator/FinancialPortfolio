using FinancialPortfolio.Domain.Entities;
using FinancialPortfolio.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialPortfolio.Domain.Events
{
    public class PortfolioUpdateEvent : IDomainEvent
    {
        public Portfolio _portfolio;
        
        public PortfolioUpdateEvent(Portfolio portfolio)
        {
            this._portfolio = portfolio;
        }
    }
}
