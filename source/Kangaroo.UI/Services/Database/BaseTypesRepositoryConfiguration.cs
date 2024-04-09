﻿using System;
using Dapper;

namespace Kangaroo.UI.Services.Database;

public sealed class BaseTypesRepositoryConfiguration : IConfigureRepository
{
    public bool Configure()
    {
        try
        {
            SqlMapper.AddTypeHandler(new GuidTypeHandler());
            SqlMapper.AddTypeHandler(new DateTimeTypeHandler());

            return true;
        }
        catch (Exception )
        {
            return false;
        }
    }
}