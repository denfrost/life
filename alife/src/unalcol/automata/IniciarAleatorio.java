package unalcol.automata;

/**
 * Created by Camilo on 14/03/2018.
 */
public class IniciarAleatorio implements Iniciar {
    @Override
    public int[][] aplicar(int n, int m, int k) {
        int[][] ac = new int[n][m];
        for( int i=0; i<n; i++ ){
            for( int j=0; j<m;j++ ){
                ac[i][j] = (int)(Math.random() * k);
            }
        }
        return ac;
    }
}
