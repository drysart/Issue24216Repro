using Issue24216Repro.Model;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace Issue24216Repro
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var ctx = new TestContext())
            {
                var q =
                    from m in ctx.Message
                    select m;

                // The .Take(10) below is required.  Without the .Take(), the problem does not occur.
                var q2 =
                    from m in q.Take(10)
                    from asof in ctx.GetPersonStatusAsOf(m.PersonId, m.Timestamp)
                    select new
                    { 
                        Gender = (from g in ctx.Gender where g.Id == asof.GenderId select g.Description).Single() 
                    };

                // The cross apply gets aliased as [g0], but the subselect generated
                // to populate the Gender property of the result refers to the cross apply as [g],
                // which conflicts with the [g] that the subselect uses to refer to the Gender table
                var qstr = q2.ToQueryString();

/* 
 
Actual output
=============
DECLARE @__p_0 int = 10;

SELECT (
    SELECT TOP(1) [g].[Description]
    FROM [Gender] AS [g]
    WHERE [g].[Id] = [g].[GenderId]) AS [Gender]
FROM (
    SELECT TOP(@__p_0) [m].[PersonId], [m].[Timestamp]
    FROM [Message] AS [m]
) AS [t]
CROSS APPLY [dbo].[GetPersonStatusAsOf]([t].[PersonId], [t].[Timestamp]) AS [g0]

Expected output
===============
DECLARE @__p_0 int = 10;

SELECT (
    SELECT TOP(1) [g].[Description]
    FROM [Gender] AS [g]
    WHERE [g].[Id] = [g0].[GenderId]) AS [Gender]   // <<<<<<< DIFFERENCE HERE
FROM (
    SELECT TOP(@__p_0) [m].[PersonId], [m].[Timestamp]
    FROM [Message] AS [m]
) AS [t]
CROSS APPLY [dbo].[GetPersonStatusAsOf]([t].[PersonId], [t].[Timestamp]) AS [g0]

*/

                Console.WriteLine(qstr);
            }

            Console.ReadLine();
        }
    }
}
