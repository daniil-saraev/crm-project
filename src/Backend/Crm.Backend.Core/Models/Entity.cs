﻿using Ardalis.GuardClauses;

namespace Crm.Backend.Core.Models
{
    public abstract class Entity
    {
        private readonly Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get { return _id; }
            init { _id = Guard.Against.NullOrEmpty(value); }
        }
    }
}
