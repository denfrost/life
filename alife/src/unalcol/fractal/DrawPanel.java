package unalcol.fractal;

import javax.swing.JPanel;
import java.awt.Graphics;
import java.util.Vector;

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

import java.awt.Color;

public class DrawPanel extends JPanel {
    protected long delay = 10;
    protected Vector points = new Vector();
    protected double[][] vertices = null;
    public DrawPanel() {
        try {
            jbInit();
        } catch (Exception ex) {
            ex.printStackTrace();
        }
    }

    public void set( Vector _points, double[][] _vertices ){
      points = _points;
      vertices = _vertices;
      invalidate();
    }

    public void paint( Graphics g ){
        int w = this.getWidth();
        int h = this.getHeight();
        int n = points.size();
        for( int i=0; i<n; i++ ){
            double[] point = (double[])points.get(i);
            int x = (int)(w * ( point[0] + 1.0 )/2.0);
            int y = h - (int)(h * ( point[1] + 1.0 )/2.0);
            g.drawLine(x,y,x,y);
        }
        g.setColor(Color.red);
        if( vertices != null ){
            for (int i = 0; i < vertices.length; i++) {
                int x = (int) (w * (vertices[i][0] + 1.0) / 2.0);
                int y = h - (int) (h * (vertices[i][1] + 1.0) / 2.0);
                g.drawLine(x-1, y-1, x+1, y+1);
            }
        }
    }

    private void jbInit() throws Exception {
        this.setBackground(Color.white);
    }
}
