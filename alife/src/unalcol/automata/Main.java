package unalcol.automata;

import java.util.Arrays;

public class Main {

    public static void main(String[] args) {
        Iniciar inicia = new IniciarAleatorio();
        Vecino v = new Dim1Vecino();
        CambioEstado f = new ReglaDim1( 2+16+128);
        Automata a = new Automata(inicia.aplicar(1,150, 2));
        System.out.print(a.imprimir());
        for( int i=0; i<100; i++){
            a.simular(v,f);
            System.out.print(a.imprimir());
        }
    }
}
