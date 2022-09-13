using System;

//*Requerimiento 1: Eliminar las doble comillas del printf e interpretar las secuencias de escape
//                  dentro de la cadena. ( Listo :o )
//*Requerimiento 2: Marcar los errores sintacticos cuando la variable no exista. ( Listo :o )
//*Requerimiento 3: Modificar el valor de la variable en la asignacion
//*Requerimiento 4: Obtener el valor de la variable cuando se requiera y programar el mètodo getValor
//*Requerimiento 5: Modificar el valor de la variable en el scanf ( Listo :o ) 

namespace Evalua
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        public Lenguaje()
        {

        }
        public Lenguaje(string nombre) : base(nombre)
        {

        }
        private void addVariable(string nombre, Variable.TipoDato tipo)
        {
            variables.Add(new Variable(nombre, tipo));
        }
        private void displayVariables()
        {
            log.WriteLine();
            log.WriteLine("Variables: ");
            foreach (Variable v in variables)
            {
                log.WriteLine(v.getNombre() + " " + v.getTipo() + " " + v.getValor());
            }
        }
        private bool existeVariable(string nombre)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    return true;
                }
            }
            return false;
        }
        //Requerimiento 3, 5-----
        private void modVariable(string nombre, float nuevoValor)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    v.setValor(nuevoValor);
                    return;
                }
            }
        }
        //Requerimiento 4---
        private float getValor(string nombreVariable)//foreach
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombreVariable))
                {
                    return v.getValor();
                }
            }
            return 0; 
        }
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            Libreria();
            Variables();
            Main();
            displayVariables();
        }

        //Librerias -> #include<identificador(.h)?> Librerias?
        private void Libreria()
        {
            if (getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(Tipos.Identificador);
                if (getContenido() == ".")
                {
                    match(".");
                    match("h");
                }
                match(">");//if (getContenido() == "#")
                Libreria();
            }
        }

        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variable.TipoDato tipo = Variable.TipoDato.Char;
                switch (getContenido())
                {
                    case "int":
                        tipo = Variable.TipoDato.Int;
                        break;
                    case "float":
                        tipo = Variable.TipoDato.Float;
                        break;
                }
                match(Tipos.TipoDato);
                Lista_identificadores(tipo);
                match(Tipos.FinSentencia);
                Variables();
            }
        }

        //Lista_identificadores -> identificador (,Lista_identificadores)?
        private void Lista_identificadores(Variable.TipoDato tipo)
        {
            if (getClasificacion() == Tipos.Identificador)
            {
                if (!existeVariable(getContenido()))
                {
                    addVariable(getContenido(), tipo);
                }
                else
                {
                    throw new Error("Error de sintaxis, variable duplicada <" + getContenido() + "> en linea: " + linea, log);
                }
            }
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                Lista_identificadores(tipo);
            }
        }
        //Bloque de instrucciones -> {lista de intrucciones?}
        private void BloqueInstrucciones()
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            Instruccion();
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase()
        {
            Instruccion();
            if (getContenido() != "case" && getContenido() != "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase();
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion()
        {
            if (getContenido() == "printf")
            {
                Printf();
            }
            else if (getContenido() == "scanf")
            {
                Scanf();
            }
            else if (getContenido() == "if")
            {
                If();
            }
            else if (getContenido() == "while")
            {
                While();
            }
            else if (getContenido() == "do")
            {
                Do();
            }
            else if (getContenido() == "for")
            {
                For();
            }
            else if (getContenido() == "switch")
            {
                Switch();
            }
            else
            {
                Asignacion();
            }
        }

        //Asignacion -> identificador = cadena | Expresion;
        private void Asignacion()
        {
            //Requerimiento 2---->si no existe la variable levantamos la excepcion
            if(!existeVariable(getContenido()))
            {
                throw new Error("La variable "+getContenido()+" no existe.", log);
            } 
            log.WriteLine();
            log.Write(getContenido() + " = ");
            string nombre = getContenido();
            match(Tipos.Identificador);
            match(Tipos.Asignacion);
            Expresion();
            match(";");
            float resultado = stack.Pop();
            log.Write(" = " + resultado);
            log.WriteLine();
            //Requerimiento 3----
            modVariable(nombre, resultado);
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While()
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do()
        {
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For()
        {
            match("for");
            match("(");
            Asignacion();
            Condicion();
            match(";");
            Incremento();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }

        //Incremento -> Identificador ++ | --
        //Funcion para requerimiento 4
        private void Incremento()
        {
            string variable = getContenido();
            //Requerimiento 2---->si no existe la variable levantamos la excepcion
            if(!existeVariable(getContenido()))
            {
                throw new Error("La variable "+getContenido()+" no existe.", log);
            } 
            match(Tipos.Identificador);
            if (getContenido() == "++") //Faltaba un signo de mas
            {
                match("++");
                modVariable(variable, getValor(variable) + 1);
            }
            else
            {
                match("--");
                modVariable(variable, getValor(variable) - 1);

            }
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch()
        {
            match("switch");
            match("(");
            Expresion();
            stack.Pop();
            match(")");
            match("{");
            ListaDeCasos();
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos()
        {
            match("case");
            Expresion();
            stack.Pop();
            match(":");
            ListaInstruccionesCase();
            if (getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if (getContenido() == "case")
            {
                ListaDeCasos();
            }
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: ) //Condicion -> Expresion operador relacional Expresion
        private void Condicion()
        {
            Expresion();
            stack.Pop();
            match(Tipos.OperadorRelacional);
            Expresion();
            stack.Pop();
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If()
        {
            match("if");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            if (getContenido() == "else")
            {
                match("else");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }
            }
        }

        //Printf -> printf(cadena);
        private void Printf()
        {
            match("printf");
            match("(");
            if (getClasificacion() == Tipos.Cadena)
            {
                //Requerimiento 1 
                setContenido(getContenido().Replace("\"","")); // El caracter "\" se usa para que nos deje tomar las comillas como caracter a remplazar.
                setContenido(getContenido().Replace("\\n","\n")); // El caracter "\\n" se usa para que nos deje tomar el salto de linea para remplazarlo.
                setContenido(getContenido().Replace("\\t","\t")); // El caracter "\\t" se usa para que nos deje tomar la tabulacion para remplazarlo.
                Console.Write(getContenido()); 
                match(Tipos.Cadena);
            }
            else
            {
                Expresion();
                Console.Write(stack.Pop());
            }
            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena, &Identificador);
        private void Scanf()
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");
            //Requerimiento 2 - Si no existe la variable levanta la excepcion
            if(!existeVariable(getContenido()))
            {
                throw new Error("La variable "+getContenido()+" no existe.", log);
            } 
            string valor = ""+Console.ReadLine();//Leemos el valor escrito en la consola y se lo asignamos a la variable valor.
            //Requerimiento 5: Modificar el valor de la variable
            modVariable(getContenido(), float.Parse(valor)); //Usamos el parse para cambiar el tipo de dato de la variable valor.
            match(Tipos.Identificador);
            match(")");
            match(";");
        }

        //Main      -> void main() Bloque de instrucciones
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones();
        }

        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                float n2 = stack.Pop();

                switch (operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        break;

                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                float n2 = stack.Pop();
                switch (operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(getContenido() + " ");
                stack.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                //Requerimiento 2---->si no existe la variable levantamos la excepcion
                if(!existeVariable(getContenido()))
                {
                    throw new Error("La variable "+getContenido()+" no existe.", log);
                } 
                log.Write(getContenido() + " ");
                stack.Push(getValor((getContenido())));
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                Expresion();
                match(")");
            }
        }
    }
}