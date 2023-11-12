using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyJoystick
{

    PlayerInput playerInput;

    public int number = 0;   //

    string type;
    public Color color;

    public string vertInput;
    public string horzInput;
    public string jumpInput;
    public string action1Input;
    public string rTriggerInput;
    public string superInput;
    public string blockInput;
    public string pauseInput;

    public string altAction1Input;
    public string altSuper1Input;
    public string altDodge1Input;
    public string altLeft1Input = "a";
    public string altRight1Input = "d";
    public string altUp1Input = "w";
    public string altDown1Input = "s";


    public bool trainingWheels = false;
    public static string mode = "Basic";


    public enum Type { ps, xbox };



    void init()
    {

        Debug.Log("joystick number = " + number);

        if (type == "Controller (Xbox One For Windows)")
        {
            InitXbox();
        }
        else
        {
            InitPlayStation();
        }

    }

    private void InitXbox()
    {
        Debug.Log("Xbox controller detected @ myJoystick");

        if (number == 0 || number == -1)
        {
            vertInput = "Vertical_P1";
            horzInput = "Horizontal_P1";
            jumpInput = "Dodge_P1_X4W";
            action1Input = "joystick 1 button 0";
            rTriggerInput = "Throw_P1_X4W";
            superInput = "Super_P1_X4W";
            blockInput = "Block_P1";
            pauseInput = "joystick 1 button 7";

            altAction1Input = "h";
            altSuper1Input = "j";
            altDodge1Input = "k";
        }
        if (number == 1)
        {
            vertInput = "Vertical_P2";
            horzInput = "Horizontal_P2";
            jumpInput = "Dodge_P2_X4W";
            action1Input = "joystick 2 button 0";
            altAction1Input = "p";
            rTriggerInput = "Throw_P2_X4W";
            superInput = "Super_P2_X4W";
            blockInput = "Block_P2";
            pauseInput = "joystick 2 button 7";

            altAction1Input = "u";
            altSuper1Input = "i";
            altDodge1Input = "o";
        }
        if (number == 2)
        {
            vertInput = "Vertical_P3";
            horzInput = "Horizontal_P3";
            jumpInput = "Dodge_P3_X4W";
            altAction1Input = "p";
            action1Input = "joystick 3 button 0";
            rTriggerInput = "Throw_P3_X4W";
            superInput = "Super_P3_X4W";
            blockInput = "Block_P3";
            pauseInput = "joystick 3 button 7";

            altAction1Input = "b";
            altSuper1Input = "n";
            altDodge1Input = "m";

        }
        if (number == 3)
        {
            vertInput = "Vertical_P4";
            horzInput = "Horizontal_P4";
            jumpInput = "Dodge_P4_X4W";
            altAction1Input = "r";
            action1Input = "joystick 4 button 0";
            rTriggerInput = "Throw_P4_X4W";
            superInput = "Super_P4_X4W";
            blockInput = "Block_P4";
            pauseInput = "joystick 4 button 7";

            altAction1Input = "1";
            altSuper1Input = "2";
            altDodge1Input = "3";

        }
    }

    private void InitPlayStation()                      // * initOther
    {

        Debug.Log("Other controller detected @ myJoystick");

        if (number == 0  || number == -1)
        {

            vertInput = "Vertical_P1";
            horzInput = "Horizontal_P1";
            jumpInput = "Jump_P1";
            action1Input = "joystick 1 button 1";
            rTriggerInput = "Fire_P1";
            superInput = "Super_P1";
            blockInput = "Block_P1";
            pauseInput = "joystick 1 button 9";

            altAction1Input = "h";
            altSuper1Input = "j";
            altDodge1Input = "k";
            altLeft1Input = "a";
            altRight1Input = "d";
            altUp1Input = "w";
            altDown1Input = "s";


            if (mode == "Basic")
            {
                trainingWheels = true;
            }
            else
            {
                trainingWheels = false;
            }
        }
        if (number == 1)
        {

            vertInput = "Vertical_P2";
            horzInput = "Horizontal_P2";
            jumpInput = "Jump_P2";
            action1Input = "joystick 2 button 1";
            rTriggerInput = "Fire_P2";
            superInput = "Super_P2";
            blockInput = "Block_P2";
            pauseInput = "joystick 2 button 9";

            altAction1Input = "u";
            altSuper1Input = "i";
            altDodge1Input = "o";

        }
        if (number == 2)
        {
            vertInput = "Vertical_P3";
            horzInput = "Horizontal_P3";
            jumpInput = "Jump_P3";
            action1Input = "joystick 3 button 1";
            rTriggerInput = "Fire_P3";
            superInput = "Super_P3";
            blockInput = "Block_P3";
            pauseInput = "joystick 3 button 9";

            altAction1Input = "b";
            altSuper1Input = "n";
            altDodge1Input = "m";

        }
        if (number == 3)
        {
            vertInput = "Vertical_P4";
            horzInput = "Horizontal_P4";
            jumpInput = "Jump_P4";
            action1Input = "joystick 4 button 1";
            rTriggerInput = "Fire_P4";
            superInput = "Super_P4";
            blockInput = "Block_P4";
            pauseInput = "joystick 4 button 9";

            altAction1Input = "1";
            altSuper1Input = "2";
            altDodge1Input = "3";
        }
    }

    public MyJoystick(int playerIndex)             // keyboard = -1
    {
        number = playerIndex;
        init();
    }

    public MyJoystick(int playerIndex, string t)
    {
        type = t;
        number = playerIndex;
        init();
    }


    public MyJoystick(PlayerInput pi, Color x)
    {
        playerInput = pi; ;
        number = pi.playerIndex;
        color = x;
        init();
    }

    public MyJoystick(int playerIndex, Color x)
    {
        number = playerIndex;
        color = x;
        init();
    }

    public MyJoystick()
    {

    }

    public void SetJoystick(int nuNumber)
    {

    }

    public void SetJoystick(int nuNumber, string type)
    {    // depreated w new input system

        number = nuNumber;

        Debug.Log("type = " + type);

        if (number == 1)
        {
            if (mode == "Basic")
            {
                trainingWheels = true;
            }
            else
            {
                trainingWheels = false;
            }
        }

        if (type == "Controller (Xbox One For Windows)")
        {

            Debug.Log("Xbox controller detected @ myJoystick");

            if (number == 1)
            {
                vertInput = "Vertical_P1";
                horzInput = "Horizontal_P1";
                jumpInput = "Dodge_P1_X4W";
                action1Input = "joystick 1 button 0";
                rTriggerInput = "Throw_P1_X4W";
                superInput = "Super_P1_X4W";
                blockInput = "Block_P1";
                pauseInput = "joystick 1 button 7";

                altAction1Input = "h";
                altSuper1Input = "j";
                altDodge1Input = "k";
            }
            if (number == 2)
            {
                vertInput = "Vertical_P2";
                horzInput = "Horizontal_P2";
                jumpInput = "Dodge_P2_X4W";
                action1Input = "joystick 2 button 0";
                altAction1Input = "p";
                rTriggerInput = "Throw_P2_X4W";
                superInput = "Super_P2_X4W";
                blockInput = "Block_P2";
                pauseInput = "joystick 2 button 7";

                altAction1Input = "u";
                altSuper1Input = "i";
                altDodge1Input = "o";
            }
            if (number == 3)
            {
                vertInput = "Vertical_P3";
                horzInput = "Horizontal_P3";
                jumpInput = "Dodge_P3_X4W";
                altAction1Input = "p";
                action1Input = "joystick 3 button 0";
                rTriggerInput = "Throw_P3_X4W";
                superInput = "Super_P3_X4W";
                blockInput = "Block_P3";
                pauseInput = "joystick 3 button 7";

                altAction1Input = "b";
                altSuper1Input = "n";
                altDodge1Input = "m";

            }
            if (number == 4)
            {
                vertInput = "Vertical_P4";
                horzInput = "Horizontal_P4";
                jumpInput = "Dodge_P4_X4W";
                altAction1Input = "r";
                action1Input = "joystick 4 button 0";
                rTriggerInput = "Throw_P4_X4W";
                superInput = "Super_P4_X4W";
                blockInput = "Block_P4";
                pauseInput = "joystick 4 button 7";

                altAction1Input = "1";
                altSuper1Input = "2";
                altDodge1Input = "3";

            }
        }

        else if (type == "Wireless Controller")
        {

            if (number == 1)
            {

                vertInput = "Vertical_P1";
                horzInput = "Horizontal_P1";
                jumpInput = "Jump_P1";
                action1Input = "joystick 1 button 1";
                rTriggerInput = "Fire_P1";
                superInput = "Super_P1";
                blockInput = "Block_P1";
                pauseInput = "joystick 1 button 9";

                altAction1Input = "h";
                altSuper1Input = "j";
                altDodge1Input = "k";
            }
            if (number == 2)
            {
                vertInput = "Vertical_P2";
                horzInput = "Horizontal_P2";
                jumpInput = "Jump_P2";
                action1Input = "joystick 2 button 1";
                rTriggerInput = "Fire_P2";
                superInput = "Super_P2";
                blockInput = "Block_P2";
                pauseInput = "joystick 2 button 9";

                altAction1Input = "u";
                altSuper1Input = "i";
                altDodge1Input = "o";

            }
            if (number == 3)
            {
                vertInput = "Vertical_P3";
                horzInput = "Horizontal_P3";
                jumpInput = "Jump_P3";
                action1Input = "joystick 3 button 1";
                rTriggerInput = "Fire_P3";
                superInput = "Super_P3";
                blockInput = "Block_P3";
                pauseInput = "joystick 3 button 9";

                altAction1Input = "b";
                altSuper1Input = "n";
                altDodge1Input = "m";

            }
            if (number == 4)
            {
                vertInput = "Vertical_P4";
                horzInput = "Horizontal_P4";
                jumpInput = "Jump_P4";
                action1Input = "joystick 4 button 1";
                rTriggerInput = "Fire_P4";
                superInput = "Super_P4";
                blockInput = "Block_P4";
                pauseInput = "joystick 4 button 9";

                altAction1Input = "1";
                altSuper1Input = "2";
                altDodge1Input = "3";
            }
        }

        else
        {
            if (number == 1)
            {
                vertInput = "Vertical_P1";
                horzInput = "Horizontal_P1";
                jumpInput = "Jump_P1";
                action1Input = "joystick 1 button 1";
                rTriggerInput = "Fire_P1";
                superInput = "Super_P1";
                blockInput = "Block_P1";
                pauseInput = "joystick 1 button 9";

                altAction1Input = "h";
                altSuper1Input = "j";
                altDodge1Input = "k";
            }
            if (number == 2)
            {
                vertInput = "Vertical_P2";
                horzInput = "Horizontal_P2";
                jumpInput = "Jump_P2";

                action1Input = "joystick 2 button 1";
                rTriggerInput = "Fire_P2";
                superInput = "Super_P2";
                blockInput = "Block_P2";
                pauseInput = "joystick 2 button 9";

                altAction1Input = "u";
                altSuper1Input = "i";
                altDodge1Input = "o";
            }
            if (number == 3)
            {
                vertInput = "Vertical_P3";
                horzInput = "Horizontal_P3";
                jumpInput = "Jump_P3";
                action1Input = "joystick 3 button 1";
                rTriggerInput = "Fire_P3";
                superInput = "Super_P3";
                blockInput = "Block_P3";
                pauseInput = "joystick 3 button 9";

                altAction1Input = "b";
                altSuper1Input = "n";
                altDodge1Input = "m";

            }
            if (number == 4)
            {
                vertInput = "Vertical_P4";
                horzInput = "Horizontal_P4";
                jumpInput = "Jump_P4";
                action1Input = "joystick 4 button 1";
                rTriggerInput = "Fire_P4";
                superInput = "Super_P4";
                blockInput = "Block_P4";
                pauseInput = "joystick 4 button 9";

                altAction1Input = "1";
                altSuper1Input = "2";
                altDodge1Input = "3";

            }
        }
    }

    internal void Print()
    {
        Debug.Log(vertInput);
        Debug.Log(horzInput);
        Debug.Log(jumpInput);
        Debug.Log(altAction1Input);
        Debug.Log(action1Input);
        Debug.Log(rTriggerInput);
        Debug.Log(superInput);
        Debug.Log(blockInput);
        Debug.Log(pauseInput);
    }
}
