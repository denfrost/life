package unalcol.automata;

/**
 * Created by Camilo on 14/03/2018.
 */
public interface CambioEstado {
    default int obtener(int i, int j, int[] x, int[][] ac){
        int n = ac.length;
        int m = ac[0].length;
        return ac[(i+x[0]+n)%n][(j+x[1]+m)%m];
    }
    int aplicar(int i, int j, int[][] ac, Vecino v);
}
