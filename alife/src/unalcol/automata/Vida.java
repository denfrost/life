package unalcol.automata;

/**
 * Created by Camilo on 14/03/2018.
 */
public class Vida implements CambioEstado {
    protected int cuenta(int i, int j, int[][] ac, Vecino v){
        int[][] vecinos = v.obtener();
        int c=0;
        for( int[] x:vecinos ){
            c += obtener(i,j,x, ac);
        }
        return c;
    }

    @Override
    public int aplicar(int i, int j, int[][] ac, Vecino v) {
        int c = cuenta(i,j,ac,v);
        int e = ac[i][j];
        if( c < 2 ) return 0;
        if(c==2) return e;
        if( c==3 ) return 1;
        return 0;
    }
}
