using Herbet_Ioana_Proiect.Models;
using Microsoft.EntityFrameworkCore;

namespace Herbet_Ioana_Proiect.Data
{
    public class DbInitializer
    {
    public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new
           DressesContext(serviceProvider.GetRequiredService
            <DbContextOptions<DressesContext>>()))
            {
                if (context.Dresses.Any())
                {
                    return; // BD a fost creata anterior

                    context.SaveChanges();
                }
            }
        }
    }
}
