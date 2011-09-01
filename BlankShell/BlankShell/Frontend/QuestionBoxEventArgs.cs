//------------------------------------------------------------------------------
// Filename: MenuEventArgs.cs
// Author: nholmes
// Summary: custom event argument handler for the menu system
//------------------------------------------------------------------------------

//**********************************************************************************
//**********************************************************************************
// NOTE this code is provided only as an example of how to use the indie city XNA 
// code. it is not part of the code base and is probably dirty, hacky and full of
// horrible bugs :P feel free to browse it, but do not judge it - most of it was
// written at this years global game jam with little or no sleep and no time to make
// it sensible or easy to follow. we'll improve this example code as and when we can
//**********************************************************************************
//**********************************************************************************

using System;
using Microsoft.Xna.Framework;

namespace BlankShell.Frontend
{
    //------------------------------------------------------------------------------
    // Class: QuestionBoxEventArgs
    // Author: nholmes
    // Summary: custom event argument which option that the user selected when they
    //          triggered the event. used by the QuestionBox.Selected event
    //------------------------------------------------------------------------------
    class QuestionBoxEventArgs : EventArgs
    {
        // local storage for the option that was selected
        SelectedOption selectedOption;

        //------------------------------------------------------------------------------
        // Function: QuestionBoxEventArgs
        // Author: nholmes
        // Summary: constructor
        //------------------------------------------------------------------------------
        public QuestionBoxEventArgs(SelectedOption selectedOption)
        {
            this.selectedOption = selectedOption;
        }

        //------------------------------------------------------------------------------
        // Function: SelectedOption
        // Author: nholmes
        // Summary: gets the option that was selected when this event was triggered
        //------------------------------------------------------------------------------
        public SelectedOption SelectedOption
        {
            get { return selectedOption; }
        }
    }
}
