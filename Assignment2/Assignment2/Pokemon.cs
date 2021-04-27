using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

enum enumPokemon {
    Pikachu,
    Charmander,
    Bulbasaur,
    Squirtle,
    Ghastly,
    Ekans,
    Eevee,
    Snorlax,
    Jigglypuff,
    Mewtwo,
    Ditto,
    Gengar,
    Dratini
}

namespace Assignment2 {

    class Pokemon {
        public static enumPokemon randPokemon() {
            Random rnd = new Random();
            return (enumPokemon)rnd.Next(0, Enum.GetNames(typeof(enumPokemon)).Length);
        }
    }
}
