﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO
{
    class Dealer
    {
        //------------------------------
        // Variables
        //------------------------------

        static Random rng = new Random();

        List<Card> deck = new List<Card>();

        //------------------------------
        // Functions
        //------------------------------

        internal void AddToDeck(Card card, bool random = false)
        {
            deck.Add(card);

            // Move it to a random position in the deck
            if (random)
            {
                var i = deck.Count - 1;

                int k = rng.Next(i + 1);

                Card swapped = deck[k];

                deck[k] = deck[i];
                deck[i] = swapped;
            }
        }

        internal void Shuffle()
        {
            var i = deck.Count;

            while (i-- > 1)
            {
                int k = rng.Next(i + 1);

                Card swapped = deck[k];

                deck[k] = deck[i];
                deck[i] = swapped;
            }
        }

        // Deal n cards to player
        internal void Deal(Player player, int n)
        {
            if (deck.Count == 0) // TODO: Reshuffle
                return;

            while (n-- > 0)
            {
                player.AddToHand(deck.Last());
                deck.Remove(deck.Last());
            }
        }

        internal Card Deal()
        {
            Card removeCard = deck.Last();
            deck.Remove(removeCard);
            return removeCard;
        }

        internal int cardsInDeck()
        {
            return deck.Count;
        }
    }
}
