using System;

namespace Net.Data.Commons.Repository.Core
{
    public interface IRepositoryMetadata
    {
         Type GetTypeId();

         Type GetDomainType();

         Type GetRepositoryInterface();
    }
}