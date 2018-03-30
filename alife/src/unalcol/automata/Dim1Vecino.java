package unalcol.automata;

/**
 * Created by Camilo on 14/03/2018.
 */
public class Dim1Vecino implements Vecino {
    @Override
    public int[][] obtener() {
        return new int[][]{{0,-1},{0,1}};
    }
}
