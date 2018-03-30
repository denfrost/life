package unalcol.automata;


public class Automata {

    private int a[][];
    private int n,m;

    public Automata(int[][] ac)
    {
        a = ac;
        n = a.length;
        m = a[0].length;
    }

    public String imprimir(){
        StringBuilder sb = new StringBuilder();
        for( int i=0; i<n; i++) {
            for (int j = 0; j < m; j++) {
                sb.append((a[i][j]==1)?'*':' ');
            }
            sb.append('\n');
        }
        return sb.toString();
    }

    public void simular( Vecino v, CambioEstado f ){
        int[][] b = new int[n][m];
        for( int i=0; i<n; i++){
            for( int j=0; j<m; j++){
                b[i][j] = f.aplicar(i,j,a, v);
            }
        }
        a=b;
    }
}