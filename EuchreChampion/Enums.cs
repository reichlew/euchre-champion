﻿namespace EuchreChampion
{
    public enum Suit
    {        
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }

    public enum CardValue
    {
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14
    }

    public enum Position
    {
        North,
        East,
        South,
        West
    }

    public enum DealType
    {
        TwoThree,
        ThreeTwo
    }

    public enum State
    {
        FirstGame,
        DealerFound,
        Dealing,
        Dealt,
        CallingTrump,
        ChoosingCard,
        Playing,
        HandOver,
        GameOver
    }
}
