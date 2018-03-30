package unalcol.automata;

/**
 * Created by Camilo on 14/03/2018.
 */
public class ReglaDim1 implements CambioEstado{
    protected int r;

    public ReglaDim1( int r ){
        this.r = r;
    }

    @Override
    public int aplicar(int i, int j, int[][] ac, Vecino v) {
        int[][] x = v.obtener();
        int z = (obtener(i,j,x[0],ac)<<2)+(ac[i][j]<<1)+obtener(i,j,x[1],ac);
        return ((r & (1<<z)) != 0)?1:0;
    }
}
