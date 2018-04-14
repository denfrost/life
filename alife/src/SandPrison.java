public class SandPrison {
    private static int[][] CA;

    private static boolean isIn(int i, int j){
        return  i >= 0 && i < 20 && j >= 0 && j < 20;
    }

    private static boolean update(int i, int j){
        CA[i][j]++;
        if (CA[i][j] >= 4) {
            CA[i][j] = 0;
            CA[i + 1][j] = isIn(i+1, j) ? CA[i + 1][j]+1: CA[i + 1][j];
            CA[i][j + 1] = isIn(i, j+1)? CA[i][j + 1]+1:CA[i][j + 1];
            CA[i - 1][j] = isIn(i-1,j)?CA[i - 1][j]+1:CA[i - 1][j]+1;
            CA[i][j - 1] = isIn(i,j-1)? CA[i][j - 1]+1:CA[i][j - 1];
            return true;
        }
        return false;
    }
    public static void main(String[] args) throws InterruptedException {
        CA = new int[20][20];

        for(int i= 0; i < 20; i++)
            for(int j = 0; j<20 ; j++)
                CA[i][j] = (int)(Math.random()*4);

        while (true){
            int i = (int)(Math.random()*20);
            int j = (int)(Math.random()*20);
            boolean flag = update(i, j);
            while (flag){
                flag = false;
                for(int i2 = 0; i2 < 20; i2++)
                    for(int j2 = 0; j2 <20 ; j2++) {
                        if (CA[i2][j2] >= 4) {
                            update(i2, j2);
                        }
                        System.out.println(CA[i2][j2]);
                    }
            }
            Thread.sleep(10);
        }

    }

}
