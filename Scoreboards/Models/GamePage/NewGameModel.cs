using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scoreboards.Models.GamePage
{
    public class NewGameModel
    {
        public string GameName { get; set; }

        public string GameDescription { get; set; }

        public string GameLogo { get; set; }
    }
}
