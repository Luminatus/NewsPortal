using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace NewsPortal.Persistence
{
    /// <summary>
    /// Szerkesztő.
    /// </summary>
    public class Editor : IdentityUser<int>
    {
        public Editor()
        { 
            Articles = new HashSet<Article>();
        }

        /* A korábban definiált tulajdonságok közül az IdentityUser<T> tartalmazza:
		 * T Id
		 * string UserName
		 * string PasswordHash (UserPassword helyett)
		 * string Email
		 * string PhoneNumber
		 * string SecurityStamp (UserChallange helyett)
		 */

        /// <summary>
        /// Teljes név.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Cím.
        /// </summary>
        public string Address { get; set; }


        /// <summary>
        /// Foglalások.
        /// </summary>
        public ICollection<Article> Articles { get; set; }
    }
}
