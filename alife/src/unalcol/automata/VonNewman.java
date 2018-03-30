package unalcol.automata;

/**
 * Created by Camilo on 14/03/2018.
 */
public class VonNewman  implements Vecino {
    @Override
    public int[][] obtener() {
        return new int[][]{{-1,0},{0,1},{1,0},{0,-1},{-1,-1},{-1,1},{1,-1},{1,1}};
    }
}
