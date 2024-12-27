using System;
using System.Data;
using Dapper;

namespace Kangaroo.UI.Database;

public class TimeSpanTypeHandler : SqlMapper.TypeHandler<TimeSpan>
{
    public override void SetValue(IDbDataParameter parameter, TimeSpan timeSpan)
    {
        parameter.Value = timeSpan.TotalMilliseconds;
    }

    public override TimeSpan Parse(object value)
    {
        if (value == null)
        {
            return TimeSpan.Zero;
        }

        if (!TimeSpan.TryParse(value.ToString(), out var timeSpan))
        {
            return TimeSpan.Zero;
        }

        return timeSpan;
    }
}