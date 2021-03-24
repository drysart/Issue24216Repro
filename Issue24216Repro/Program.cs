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

                Console.WriteLine(qstr);
            }

            Console.ReadLine();
        }
    }
}
