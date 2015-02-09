﻿using System;
using System.IO;
using System.Text;

/* Originally written in 'C', this code has been converted to the C# language.
 * The author's copyright message is reproduced below.
 * All modifications from the original to C# are placed in the public domain.
 */

/* jsmin.c
   2007-05-22

Copyright (c) 2002 Douglas Crockford  (www.crockford.com)

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

The Software shall be used for Good, not Evil.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace ClientDependency.Core.CompositeFiles
{
    public class JSMin
    {
        private const int EOF = -1;

        private StringReader _sr;
        private StringWriter _sw;
        private int _theA;
        private int _theB;
        private int _theLookahead = EOF;
        private static int s_theX = EOF;
        private static int s_theY = EOF;

        public static string CompressJS(string body)
        {
            return new JSMin().Minify(body);
        }

        public string Minify(string src)
        {
            StringBuilder sb = new StringBuilder();
            using (_sr = new StringReader(src))
            {
                using (_sw = new StringWriter(sb))
                {
                    jsmin();
                }
            }
            return sb.ToString();
        }

        /* jsmin -- Copy the input to the output, deleting the characters which are
                insignificant to JavaScript. Comments will be removed. Tabs will be
                replaced with spaces. Carriage returns will be replaced with linefeeds.
                Most spaces and linefeeds will be removed.
        */
        private void jsmin()
        {
            if (peek() == 0xEF)
            {
                get();
                get();
                get();
            }
            _theA = '\n';
            action(3);
            while (_theA != EOF)
            {
                switch (_theA)
                {
                    case ' ':
                        action(isAlphanum(_theB) ? 1 : 2);
                        break;
                    case '\n':
                        switch (_theB)
                        {
                            case '{':
                            case '[':
                            case '(':
                            case '+':
                            case '-':
                            case '!':
                            case '~':
                                action(1);
                                break;
                            case ' ':
                                action(3);
                                break;
                            default:
                                action(isAlphanum(_theB) ? 1 : 2);
                                break;
                        }
                        break;
                    default:
                        switch (_theB)
                        {
                            case ' ':
                                action(isAlphanum(_theA) ? 1 : 3);
                                break;
                            case '\n':
                                switch (_theA)
                                {
                                    case '}':
                                    case ']':
                                    case ')':
                                    case '+':
                                    case '-':
                                    case '"':
                                    case '\'':
                                    case '`':
                                        action(1);
                                        break;
                                    default:
                                        action(isAlphanum(_theA) ? 1 : 3);
                                        break;
                                }
                                break;
                            default:
                                action(1);
                                break;
                        }
                        break;
                }
            }
        }
        /* action -- do something! What you do is determined by the argument:
                1   Output A. Copy B to A. Get the next B.
                2   Copy B to A. Get the next B. (Delete A).
                3   Get the next B. (Delete B).
           action treats a string as a single character. Wow!
           action recognizes a regular expression if it is preceded by ( or , or =.
        */
        private void action(int d)
        {
            switch (d)
            {
                case 1:
                    put(_theA);
                    if (
                        (s_theY == '\n' || s_theY == ' ') &&
                        (_theA == '+' || _theA == '-' || _theA == '*' || _theA == '/') &&
                        (_theB == '+' || _theB == '-' || _theB == '*' || _theB == '/')
                        )
                    {
                        put(s_theY);
                    }
                    goto case 2;
                case 2:
                    _theA = _theB;
                    if (_theA == '\'' || _theA == '"' || _theA == '`')
                    {
                        for (; ;)
                        {
                            put(_theA);
                            _theA = get();
                            if (_theA == _theB)
                            {
                                break;
                            }
                            if (_theA == '\\')
                            {
                                put(_theA);
                                _theA = get();
                            }
                            if (_theA == EOF)
                            {
                                throw new Exception(string.Format("Error: JSMIN unterminated string literal: {0}\n", _theA));
                            }
                        }
                    }
                    goto case 3;
                case 3:
                    _theB = next();
                    if (_theB == '/' && (
                                           _theA == '(' || _theA == ',' || _theA == '=' || _theA == ':' ||
                                           _theA == '[' || _theA == '!' || _theA == '&' || _theA == '|' ||
                                           _theA == '?' || _theA == '+' || _theA == '-' || _theA == '~' ||
                                           _theA == '*' || _theA == '/' || _theA == '{' || _theA == '\n'
                                       ))
                    {
                        put(_theA);
                        if (_theA == '/' || _theA == '*')
                        {
                            put(' ');
                        }
                        put(_theB);
                        for (; ;)
                        {
                            _theA = get();
                            if (_theA == '[')
                            {
                                for (; ;)
                                {
                                    put(_theA);
                                    _theA = get();
                                    if (_theA == ']')
                                    {
                                        break;
                                    }
                                    if (_theA == '\\')
                                    {
                                        put(_theA);
                                        _theA = get();
                                    }
                                    if (_theA == EOF)
                                    {
                                        throw new Exception(string.Format("Error: JSMIN Unterminated set in Regular Expression literal: {0}\n", _theA));
                                    }
                                }
                            }
                            else if (_theA == '/')
                            {
                                switch (peek())
                                {
                                    case '/':
                                    case '*':
                                        throw new Exception(string.Format("Error: JSMIN Unterminated set in Regular Expression literal: {0}\n", _theA));
                                }
                                break;
                            }
                            else if (_theA == '\\')
                            {
                                put(_theA);
                                _theA = get();
                            }
                            if (_theA == EOF)
                            {
                                throw new Exception(string.Format("Error: JSMIN Unterminated Regular Expression literal: {0}\n", _theA));
                            }
                            put(_theA);
                        }
                        _theB = next();
                    }
                    goto default;
                default:
                    break;
            }
        }
        /* next -- get the next character, excluding comments. peek() is used to see
                if a '/' is followed by a '/' or '*'.
        */

        private int next()
        {
            int c = get();
            if (c == '/')
            {
                switch (peek())
                {
                    case '/':
                        for (; ;)
                        {
                            c = get();
                            if (c <= '\n')
                            {
                                break;
                            }
                        }
                        break;
                    case '*':
                        get();
                        while (c != ' ')
                        {
                            switch (get())
                            {
                                case '*':
                                    if (peek() == '/')
                                    {
                                        get();
                                        c = ' ';
                                    }
                                    break;
                                case EOF:
                                    throw new Exception("Error: JSMIN Unterminated comment.\n");
                            }
                        }
                        break;
                }
            }
            //return c;
            s_theY = s_theX;
            s_theX = c;
            return c;
        }

        /* peek -- get the next character without getting it.
        */
        private int peek()
        {
            _theLookahead = get();
            return _theLookahead;
        }
        /* get -- return the next character from stdin. Watch out for lookahead. If
                the character is a control character, translate it to a space or
                linefeed.
        */
        private int get()
        {
            int c = _theLookahead;
            _theLookahead = EOF;
            if (c == EOF)
            {
                c = _sr.Read();
            }
            if (c >= ' ' || c == '\n' || c == EOF)
            {
                return c;
            }
            if (c == '\r')
            {
                return '\n';
            }
            return ' ';
        }
        private void put(int c)
        {
            _sw.Write((char)c);
        }
        /* isAlphanum -- return true if the character is a letter, digit, underscore,
                dollar sign, or non-ASCII character.
        */
        private bool isAlphanum(int c)
        {
            return ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
                    (c >= 'A' && c <= 'Z') || c == '_' || c == '$' || c == '\\' ||
                    c > 126);
        }
    }

    //public class JSMin
    //{
    //    const int EOF = -1;

    //    StringReader sr;
    //    StringWriter sw;
    //    int theA;
    //    int theB;
    //    int theLookahead = EOF;

    //    public static string CompressJS(string body)
    //    {
    //        return new JSMin().Minify(body);
    //    }

    //    public string Minify(string src)
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        using (sr = new StringReader(src))
    //        {                
    //            using (sw = new StringWriter(sb))
    //            {
    //                jsmin();
    //            }
    //        }
    //        return sb.ToString();
    //    }

    //    /* jsmin -- Copy the input to the output, deleting the characters which are
    //            insignificant to JavaScript. Comments will be removed. Tabs will be
    //            replaced with spaces. Carriage returns will be replaced with linefeeds.
    //            Most spaces and linefeeds will be removed.
    //    */
    //    void jsmin()
    //    {
    //        theA = '\n';
    //        action(3);
    //        while (theA != EOF)
    //        {
    //            switch (theA)
    //            {
    //                case ' ':
    //                    {
    //                        if (isAlphanum(theB))
    //                        {
    //                            action(1);
    //                        }
    //                        else
    //                        {
    //                            action(2);
    //                        }
    //                        break;
    //                    }
    //                case '\n':
    //                    {
    //                        switch (theB)
    //                        {
    //                            case '{':
    //                            case '[':
    //                            case '(':
    //                            case '+':
    //                            case '-':
    //                                {
    //                                    action(1);
    //                                    break;
    //                                }
    //                            case ' ':
    //                                {
    //                                    action(3);
    //                                    break;
    //                                }
    //                            default:
    //                                {
    //                                    if (isAlphanum(theB))
    //                                    {
    //                                        action(1);
    //                                    }
    //                                    else
    //                                    {
    //                                        action(2);
    //                                    }
    //                                    break;
    //                                }
    //                        }
    //                        break;
    //                    }
    //                default:
    //                    {
    //                        switch (theB)
    //                        {
    //                            case ' ':
    //                                {
    //                                    if (isAlphanum(theA))
    //                                    {
    //                                        action(1);
    //                                        break;
    //                                    }
    //                                    action(3);
    //                                    break;
    //                                }
    //                            case '\n':
    //                                {
    //                                    switch (theA)
    //                                    {
    //                                        case '}':
    //                                        case ']':
    //                                        case ')':
    //                                        case '+':
    //                                        case '-':
    //                                        case '"':
    //                                        case '\'':
    //                                            {
    //                                                action(1);
    //                                                break;
    //                                            }
    //                                        default:
    //                                            {
    //                                                if (isAlphanum(theA))
    //                                                {
    //                                                    action(1);
    //                                                }
    //                                                else
    //                                                {
    //                                                    action(3);
    //                                                }
    //                                                break;
    //                                            }
    //                                    }
    //                                    break;
    //                                }
    //                            default:
    //                                {
    //                                    action(1);
    //                                    break;
    //                                }
    //                        }
    //                        break;
    //                    }
    //            }
    //        }
    //    }
    //    /* action -- do something! What you do is determined by the argument:
    //            1   Output A. Copy B to A. Get the next B.
    //            2   Copy B to A. Get the next B. (Delete A).
    //            3   Get the next B. (Delete B).
    //       action treats a string as a single character. Wow!
    //       action recognizes a regular expression if it is preceded by ( or , or =.
    //    */
    //    void action(int d)
    //    {
    //        if (d <= 1)
    //        {
    //            put(theA);
    //        }
    //        if (d <= 2)
    //        {
    //            theA = theB;
    //            if (theA == '\'' || theA == '"')
    //            {
    //                for (; ; )
    //                {
    //                    put(theA);
    //                    theA = get();
    //                    if (theA == theB)
    //                    {
    //                        break;
    //                    }
    //                    if (theA <= '\n')
    //                    {
    //                        throw new Exception(string.Format("Error: JSMIN unterminated string literal: {0}\n", theA));
    //                    }
    //                    if (theA == '\\')
    //                    {
    //                        put(theA);
    //                        theA = get();
    //                    }
    //                }
    //            }
    //        }
    //        if (d <= 3)
    //        {
    //            theB = next();
    //            if (theB == '/' && (theA == '(' || theA == ',' || theA == '=' ||
    //                                theA == '[' || theA == '!' || theA == ':' ||
    //                                theA == '&' || theA == '|' || theA == '?' ||
    //                                theA == '{' || theA == '}' || theA == ';' ||
    //                                theA == '\n'))
    //            {
    //                put(theA);
    //                put(theB);
    //                for (; ; )
    //                {
    //                    theA = get();
    //                    if (theA == '/')
    //                    {
    //                        break;
    //                    }
    //                    else if (theA == '\\')
    //                    {
    //                        put(theA);
    //                        theA = get();
    //                    }
    //                    else if (theA <= '\n')
    //                    {
    //                        throw new Exception(string.Format("Error: JSMIN unterminated Regular Expression literal : {0}.\n", theA));
    //                    }
    //                    put(theA);
    //                }
    //                theB = next();
    //            }
    //        }
    //    }
    //    /* next -- get the next character, excluding comments. peek() is used to see
    //            if a '/' is followed by a '/' or '*'.
    //    */
    //    int next()
    //    {
    //        int c = get();
    //        if (c == '/')
    //        {
    //            switch (peek())
    //            {
    //                case '/':
    //                    {
    //                        for (; ; )
    //                        {
    //                            c = get();
    //                            if (c <= '\n')
    //                            {
    //                                return c;
    //                            }
    //                        }
    //                    }
    //                case '*':
    //                    {
    //                        get();
    //                        for (; ; )
    //                        {
    //                            switch (get())
    //                            {
    //                                case '*':
    //                                    {
    //                                        if (peek() == '/')
    //                                        {
    //                                            get();
    //                                            return ' ';
    //                                        }
    //                                        break;
    //                                    }
    //                                case EOF:
    //                                    {
    //                                        throw new Exception("Error: JSMIN Unterminated comment.\n");
    //                                    }
    //                            }
    //                        }
    //                    }
    //                default:
    //                    {
    //                        return c;
    //                    }
    //            }
    //        }
    //        return c;
    //    }
    //    /* peek -- get the next character without getting it.
    //    */
    //    int peek()
    //    {
    //        theLookahead = get();
    //        return theLookahead;
    //    }
    //    /* get -- return the next character from stdin. Watch out for lookahead. If
    //            the character is a control character, translate it to a space or
    //            linefeed.
    //    */
    //    int get()
    //    {
    //        int c = theLookahead;
    //        theLookahead = EOF;
    //        if (c == EOF)
    //        {
    //            c = sr.Read();
    //        }
    //        if (c >= ' ' || c == '\n' || c == EOF)
    //        {
    //            return c;
    //        }
    //        if (c == '\r')
    //        {
    //            return '\n';
    //        }
    //        return ' ';
    //    }
    //    void put(int c)
    //    {
    //        sw.Write((char)c);
    //    }
    //    /* isAlphanum -- return true if the character is a letter, digit, underscore,
    //            dollar sign, or non-ASCII character.
    //    */
    //    bool isAlphanum(int c)
    //    {
    //        return ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
    //            (c >= 'A' && c <= 'Z') || c == '_' || c == '$' || c == '\\' || c == '+' ||
    //            c > 126);
    //    }
    //}
}
