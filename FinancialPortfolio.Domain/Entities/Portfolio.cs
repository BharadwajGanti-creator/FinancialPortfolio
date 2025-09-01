using FinancialPortfolio.Domain.Events;
using FinancialPortfolio.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FinancialPortfolio.Domain.Entities
{
    public class Portfolio
    {
        public Guid PortfolioId { get; private set; }
        public string OwnerId { get; private set; }

        public string Name { get; private set; }

        public DateTime CreationDate { get; private set; }

        public decimal TotalValue { get; private set; }

        public DateTime? LastCalculatedAt { get; private set; }

        public bool IsDirty { get; private set; }

        private List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public Portfolio(Guid PortfolioId, string OwnerId, string Name, DateTime CreationDate, decimal TotalValue, DateTime? LastCalculatedAt, bool IsDirty)
        {
            
                this.PortfolioId = PortfolioId;
                this.OwnerId = OwnerId;
                this.Name = Name;
                this.CreationDate = CreationDate;
                this.TotalValue = TotalValue;
                this.LastCalculatedAt = LastCalculatedAt;
                this.IsDirty = IsDirty;

                if (string.IsNullOrEmpty(Name))
                    throw new InvalidPortfolioNameException(nameof(Name));
                if (TotalValue < 0)
                throw new InvalidPortfolioValueException(nameof(TotalValue));
                   
        }
        /// <summary>
        /// Recalculates the Net Asset Value (NAV) and updates the calculation timestamp.
        /// </summary>
        /// <remarks>This method resets the <see cref="IsDirty"/> flag to <see langword="false"/> if it is
        /// set,  and updates the <see cref="LastCalculatedAt"/> property to the current UTC time.</remarks>
        public void CalculateNAV()
        {
            if (IsDirty)
                IsDirty = false;
            LastCalculatedAt = DateTime.UtcNow;
        }
        /// <summary>
        /// Marks the current object as dirty, indicating that it has been modified.
        /// </summary>
        /// may also trigger domain events or other side effects related to the object's state change.</remarks>
        public void MarkAsDirty()
        {
            IsDirty = true;
            //Emitting the Compliance Handler Domain Event to subscribe to the required events
            _domainEvents.Add(new PortfolioUpdateEvent(this));
        }
        /// <summary>
        /// Clears all domain events associated with the current entity.
        /// </summary>
        /// <remarks>This method removes all domain events from the internal collection, effectively
        /// resetting the entity's event state. It is typically used after the events have been processed or
        /// dispatched.</remarks>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
       /// <summary>
       /// Updates the name of the portfolio if the specified name is valid and different from the current name.
       /// </summary>
       /// <remarks>If the new name is the same as the current name, no changes are made. If the name is
       /// updated, the portfolio is marked as modified.</remarks>
       /// <param name="newName">The new name to assign to the portfolio. Must be between 1 and 32 characters long and contain only valid
       /// characters.</param>
       /// <exception cref="InvalidPortfolioNameException">Thrown if <paramref name="newName"/> is null, empty, outside the valid length range (1-32 characters), or
       /// contains invalid characters.</exception>
        public void UpdateName(string newName)
        {
            if (string.IsNullOrEmpty(newName))
                throw new InvalidPortfolioNameException(nameof(newName));
            if (newName.Length < 1 || newName.Length > 32)
                throw new InvalidPortfolioNameException(nameof(newName));
            if (!IsValidCharacters(newName))
                throw new InvalidPortfolioNameException(nameof(newName));
            if (!Name.Equals(newName))
            {
                Name = newName;
                IsDirty = true;
            }
            _domainEvents.Add(new PortfolioUpdateEvent(this));
        }
        /// <summary>
        /// Determines whether the specified string contains only valid characters.
        /// </summary>
        public bool IsValidCharacters(string newName)
        {
            char[] forbiddenCharacters = { '@', '/', '!', '#', '$', '%', '^', '&', '*', '(', ')', ',', '+', '=', ':', '<', '>', '/', ',', '\'', '{', '}' };
            return !newName.Any(c => forbiddenCharacters.Contains(c));
        }

    }
}
