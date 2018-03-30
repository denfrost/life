package unalcol.fractal;

import java.util.Vector;
import javax.swing.*;
import java.awt.BorderLayout;

/**
 * <p>Title: Alife-Fractal Project</p>
 *
 * <p>Description: Showing Life is an attractor (fractal) of a chaotic
 * system</p>
 *
 * <p>Copyright: Copyright (c) 2006</p>
 *
 * <p>Company: Universidad Nacional de Colombia</p>
 *
 * @author Jonatan Gomez
 * @version 1.0
 */
public class Sierpinski {
    protected boolean random_angle = false;
    protected boolean random_radius = false;
    protected double alpha = 0.5;
    protected double[][] vertices;
    public Sierpinski( int n_vertices, double alpha, boolean random_angle, boolean random_radius ) {
      this.alpha = alpha;
      this.random_angle = random_angle;
      this.random_radius = random_radius;
        set( n_vertices );
    }

    public void set( int n_vertices ) {
      vertices = new double[n_vertices][2];
      for (int k = 0; k < n_vertices; k++) {
        double angle = (random_angle) ? 2.0 * Math.PI * Math.random() :
            (2.0 * Math.PI * k) / n_vertices + Math.PI / 2.0;
        double x = Math.cos(angle);
        double y = Math.sin(angle);
        double radius = (random_radius) ? Math.random() : 1.0;
        vertices[k][0] = x * radius;
        vertices[k][1] = y * radius;
      }
    }

    public double[] next( double[] current ){
//     alpha = Math.random();
      int k = (int)(Math.random()*vertices.length);
      double x = (1.0-alpha)*current[0] + vertices[k][0]*alpha;
      double y = (1.0-alpha)*current[1] + vertices[k][1]*alpha;
      return new double[]{x,y};
    }

/*    public static void ellipse(){
        double a = 148.0;
        double b = 100.0;
        for( double y=0; y<=100; y+=1){
            double r = y/b;
            double x = a*Math.sqrt(1.0-r*r);
            double z = 2.0*Math.PI*(10.0+a-x);
            double delta = ( 992.74 - z ) / 2.0;
            double ry = 100 - y + 5;
            System.out.println( z + " " + ry + " " + delta);
        }
  }
*/
    public static void main( String[] args ){
      //System.out.println( args[0] );
      int k = 20; // Integer.parseInt(args[0]);
      int startAlpha = 30; // Integer.parseInt(args[1]);
      int finalAlpha = 90; // Integer.parseInt(args[2]);
      int deltaAlpha = 5; //Integer.parseInt(args[3]);
      boolean random_angle = true; // args[4].equals("true");
      boolean random_radius = false; // args[5].equals("true");
      int iterations = 10000; // Integer.parseInt(args[6]);

      for( int alpha = startAlpha; alpha<=finalAlpha; alpha+=deltaAlpha ){
        JFrame jf = new JFrame("Sierpinski: " + alpha);
        jf.setSize(300, 320);
        DrawPanel draw = new DrawPanel();
        jf.getContentPane().add(draw, BorderLayout.CENTER);
        jf.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        jf.setVisible(true);
//        ellipse();
        Sierpinski a = new Sierpinski(k, (double) alpha / 100.0, random_angle, random_radius);
        Vector vector = new Vector();
        double[] current = new double[] {
            0, 0};
        for (int i = 0; i < iterations; i++) {
          double[] next = a.next(current);
//            System.out.println("("+next[0]+","+next[1]+")");
          vector.add(next);
          draw.set(vector, a.vertices);
          if (i % 5 == 0) {
            try {
              Thread.sleep(1);
            }
            catch (Exception e) {}
          }
          draw.updateUI();
          current = next;
        }
      }
    }
}
