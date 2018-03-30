package unalcol.reactiondiffusion;

import java.awt.Color;
import java.awt.Component;
import java.awt.Container;
import java.awt.Graphics;
import java.awt.event.MouseEvent;
import java.awt.image.BufferedImage;
import java.text.NumberFormat;
import java.text.ParseException;
import java.util.Random;
import javax.swing.*;
import javax.swing.BorderFactory;
import javax.swing.DefaultComboBoxModel;
import javax.swing.JApplet;
import javax.swing.JColorChooser;
import javax.swing.JComponent;
import javax.swing.JTextField;
import javax.swing.SwingUtilities;
import javax.swing.UIManager;
import javax.swing.border.Border;

/**
 * An applet to demonstrate Turing's reaction-diffusion model.
 * For more information, see:
 *   [1] Rafael Collantes. Algorithm Alley. Dr. Dobb's Journal, December 1996.
 *   [2] Alan M. Turing. The chemical basis of morphogenesis. Philosophical
 *       Transactions of the Royal Society of London. B 327, 37�72 (1952)
 *
 * @author Christopher G. Jennings (cjennings [at] acm.org)
 */
public class TuringMorph extends JFrame {
       public static void main( String[] args ){
         TuringMorph w = new TuringMorph("Turing Morph" );
         w.setSize( 800, 600 );
         w.setVisible(true);
       }

        public TuringMorph( String name ){
          super( name );
          init();
        }

	/**
	 * Labels that describe the sizes of the Turing systems that the user
	 * can generate. These labels are added to the size drop down and
	 * when a new size is selected, it is read directly from the label.
	 * You may add the size n x n by adding a new string that ends with
	 * " n)" (the textures are always assumed to be square). If you add
	 * an entry larger than the current maximum size, you must also update
	 * <code>MAXIMUM_SYSTEM_SIZE</code>.
	 */
	private static final String[] SYSTEM_SIZES = new String[] {
		"Tiny (64 x 64)", "Small (128 x 128)", "Medium (192 x 192)",
		"Large (256 x 256)", "Extra Large (350 x 350)"
	};
	/**
	 * The size of the largest possible data set. This must be the largest of all
	 * the size settings in <code>imageSizeDropDown</code>.
	 */
	private static final int MAXIMUM_SYSTEM_SIZE = 350;
	/** Names for the built-in examples. */
	private static final String[] PRESET_NAMES = new String[] {
		"Cheetah", "Colony", "Fine", "Fingerprint", "Maze", "Pocked",
	};
	/** Constant pairs for the built-in examples. */
	private static final double[] PRESET_CONSTANTS = new double[] {
		3.5d, 16d, 1.6d, 6d, 0.1d, 1d, 1d, 16d, 2.6d, 24d, 1d, 3d,
	};

	/**
	 * Initializes the applet when the web browser first loads it in.
	 */
	public void init() {
		try {
			java.awt.EventQueue.invokeAndWait( new Runnable() {
				public void run() {
					try {
						UIManager.setLookAndFeel( "com.sun.java.swing.plaf.nimbus.NimbusLookAndFeel" );
					} catch( ClassNotFoundException e ) {
						// use default, Nimbus not available
					} catch( Exception e ) {
						e.printStackTrace();
					}
					initComponents();
				}
			} );
		} catch( Exception ex ) {
			ex.printStackTrace();
		}
	}

	/**
	 * Start solving for the default constants when the applet starts.
	 */
	public void start() {
		try {
			java.awt.EventQueue.invokeLater( new Runnable() {
				public void run() {
					okButtonActionPerformed( null );
				}
			} );
		} catch( Exception ex ) {
			ex.printStackTrace();
		}
	}

	/**
	 * Stop solver thread, if running, when browser leaves the page the applet is on.
	 */
	public void stop() {
		if( solver != null ) {
			// does not have to be called from EDT
			solver.stop();
		}
	}

	/**
	 * If run in Java 6 update 10 or newer, this allows the applet to be dragged
	 * out of the web page as an independent window by dragging the title bar.
	 *
	 * @param e the <code>MouseEvent</code> to test the component with
	 * @return <code>true</code> if Alt+left click is down or the title bar is clicked
	 */
	public boolean isAppletDragStart( MouseEvent e ) {
		Object source = e.getSource();

		// since this method was called, the applet is draggable
		// tell the user how to drag it out of the web page
		if( source == this ) {
			if( titleLabel.getToolTipText().equals( ABOUT_TIP ) ) {
				titleLabel.setToolTipText( ABOUT_TIP + DRAG_TIP );
				titlePanel.setToolTipText( ABOUT_TIP + DRAG_TIP );
			}
		}

		if( (e.getModifiersEx() & MouseEvent.BUTTON1_DOWN_MASK) != 0 ) {
			// check if dragging title bar
			// e.getSource() is the applet when it is embedded in the web page
			//   and a parent of the applet when it is in an independent window
			if( source == this || ((Container) source).isAncestorOf( this ) || source == titlePanel || source == titleLabel ) {
				if( e.getY() <= titlePanel.getHeight() ) {
					return true;
				}
			}
			// always drag if ALT is held down
			return (e.getModifiersEx() & MouseEvent.ALT_DOWN_MASK) != 0;
		}
		return false;
	}

    private void initComponents() {
        java.awt.GridBagConstraints gridBagConstraints;

        javax.swing.JPanel controlPanel = new javax.swing.JPanel();
        javax.swing.JLabel imageSizeLabel = new javax.swing.JLabel();
        imageSizeDropDown = new javax.swing.JComboBox();
        iterationsLabel = new javax.swing.JLabel();
        iterationsSlider = new javax.swing.JSlider();
        javax.swing.JLabel constantsLabel = new javax.swing.JLabel();
        constAField = new javax.swing.JTextField();
        constBField = new javax.swing.JTextField();
        okButton = new javax.swing.JButton();
        javax.swing.JLabel colourLabel = new javax.swing.JLabel();
        minColourButton = new javax.swing.JButton();
        maxColourButton = new javax.swing.JButton();
        progressBar = new javax.swing.JProgressBar();
        randomColourButton = new javax.swing.JButton();
        tileCheckBox = new javax.swing.JCheckBox();
        presetsLabel = new javax.swing.JLabel();
        presetDropDown = new javax.swing.JComboBox();
        randomizeCheck = new javax.swing.JCheckBox();
        randomizeNowBtn = new javax.swing.JButton();
        javax.swing.JLabel appearanceLabel = new javax.swing.JLabel();
        javax.swing.JSeparator appearanceSeparator = new javax.swing.JSeparator();
        javax.swing.JLabel advancedLabel = new javax.swing.JLabel();
        javax.swing.JSeparator advancedSeparator = new javax.swing.JSeparator();
        javax.swing.JLabel basicLabel = new javax.swing.JLabel();
        javax.swing.JSeparator basicSeparator = new javax.swing.JSeparator();
        javax.swing.JPanel visualizerPanel = new javax.swing.JPanel();
        titlePanel = new javax.swing.JPanel();
        titleLabel = new javax.swing.JLabel();

        controlPanel.setBackground(java.awt.Color.white);
        controlPanel.setBorder(javax.swing.BorderFactory.createLineBorder(new java.awt.Color(0, 0, 0)));
        controlPanel.setLayout(new java.awt.GridBagLayout());

        imageSizeLabel.setDisplayedMnemonic('S');
        imageSizeLabel.setLabelFor(imageSizeDropDown);
        imageSizeLabel.setText("Size (in Pixels)");
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 13;
        gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
        gridBagConstraints.insets = new java.awt.Insets(8, 8, 4, 8);
        controlPanel.add(imageSizeLabel, gridBagConstraints);

        imageSizeDropDown.setModel( new javax.swing.DefaultComboBoxModel( SYSTEM_SIZES ) );
        imageSizeDropDown.setToolTipText("Select a Size for the Texture to be Created");
        imageSizeDropDown.setOpaque(false);
        imageSizeDropDown.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                imageSizeDropDownActionPerformed(evt);
            }
        });
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 1;
        gridBagConstraints.gridy = 13;
        gridBagConstraints.gridwidth = 2;
        gridBagConstraints.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
        gridBagConstraints.weightx = 1.0;
        gridBagConstraints.insets = new java.awt.Insets(8, 0, 0, 8);
        controlPanel.add(imageSizeDropDown, gridBagConstraints);

        iterationsLabel.setDisplayedMnemonic('N');
        iterationsLabel.setLabelFor(iterationsSlider);
        iterationsLabel.setText( "Number of Iterations: " + formatter.format( 2000 ) );
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 7;
        gridBagConstraints.gridwidth = 3;
        gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
        gridBagConstraints.insets = new java.awt.Insets(8, 8, 4, 8);
        controlPanel.add(iterationsLabel, gridBagConstraints);

        iterationsSlider.setBackground(java.awt.Color.white);
        iterationsSlider.setMajorTickSpacing(4999);
        iterationsSlider.setMaximum(5000);
        iterationsSlider.setMinimum(1);
        iterationsSlider.setMinorTickSpacing(500);
        iterationsSlider.setPaintLabels(true);
        iterationsSlider.setPaintTicks(true);
        iterationsSlider.setValue(2000);
        iterationsSlider.setOpaque(false);
        iterationsSlider.addChangeListener(new javax.swing.event.ChangeListener() {
            public void stateChanged(javax.swing.event.ChangeEvent evt) {
                iterationsSliderStateChanged(evt);
            }
        });
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 8;
        gridBagConstraints.gridwidth = 3;
        gridBagConstraints.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints.insets = new java.awt.Insets(0, 16, 0, 8);
        controlPanel.add(iterationsSlider, gridBagConstraints);

        constantsLabel.setDisplayedMnemonic('C');
        constantsLabel.setLabelFor(constAField);
        constantsLabel.setText("Diffusion Constants");
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 2;
        gridBagConstraints.gridwidth = 2;
        gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
        gridBagConstraints.insets = new java.awt.Insets(8, 8, 2, 8);
        controlPanel.add(constantsLabel, gridBagConstraints);

        constAField.setColumns(4);
        constAField.setHorizontalAlignment(javax.swing.JTextField.RIGHT);
        constAField.setText( formatter.format( PRESET_CONSTANTS[0] ) );
        constAField.setMinimumSize(new java.awt.Dimension(32, 20));
        constAField.setOpaque(false);
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 3;
        gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
        gridBagConstraints.insets = new java.awt.Insets(0, 16, 0, 0);
        controlPanel.add(constAField, gridBagConstraints);

        constBField.setColumns(4);
        constBField.setHorizontalAlignment(javax.swing.JTextField.RIGHT);
        constBField.setText( formatter.format( PRESET_CONSTANTS[1] ) );
        constBField.setMinimumSize(new java.awt.Dimension(32, 20));
        constBField.setOpaque(false);
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 1;
        gridBagConstraints.gridy = 3;
        gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
        gridBagConstraints.insets = new java.awt.Insets(0, 1, 0, 4);
        controlPanel.add(constBField, gridBagConstraints);

        okButton.setMnemonic('D');
        okButton.setText("   Do It!   ");
        okButton.setToolTipText("Create a Texture Using These Settings");
        okButton.setOpaque(false);
        okButton.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                okButtonActionPerformed(evt);
            }
        });
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 2;
        gridBagConstraints.gridy = 4;
        gridBagConstraints.anchor = java.awt.GridBagConstraints.EAST;
        gridBagConstraints.insets = new java.awt.Insets(10, 16, 4, 8);
        controlPanel.add(okButton, gridBagConstraints);

        colourLabel.setText("Colours (click on a colour to change it)");
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 14;
        gridBagConstraints.gridwidth = 3;
        gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
        gridBagConstraints.insets = new java.awt.Insets(14, 8, 2, 8);
        controlPanel.add(colourLabel, gridBagConstraints);

        minColourButton.setBackground( TuringSystemVisualizer.DEFAULT_CMIN );
        minColourButton.setText("        ");
        minColourButton.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                colourButtonActionPerformed(evt);
            }
        });
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 15;
        gridBagConstraints.insets = new java.awt.Insets(0, 16, 0, 0);
        controlPanel.add(minColourButton, gridBagConstraints);

        maxColourButton.setBackground( TuringSystemVisualizer.DEFAULT_CMAX );
        maxColourButton.setText("        ");
        maxColourButton.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                colourButtonActionPerformed(evt);
            }
        });
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 1;
        gridBagConstraints.gridy = 15;
        gridBagConstraints.insets = new java.awt.Insets(0, 8, 0, 0);
        controlPanel.add(maxColourButton, gridBagConstraints);

        progressBar.setMaximum(2000);
        progressBar.setString("0");
        progressBar.setStringPainted(true);
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 4;
        gridBagConstraints.gridwidth = 2;
        gridBagConstraints.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints.weightx = 1.0;
        gridBagConstraints.insets = new java.awt.Insets(10, 8, 4, 8);
        controlPanel.add(progressBar, gridBagConstraints);

        randomColourButton.setFont(randomColourButton.getFont().deriveFont(randomColourButton.getFont().getSize()-2f));
        randomColourButton.setText("Pick Randomly");
        randomColourButton.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                randomColourButtonActionPerformed(evt);
            }
        });
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 2;
        gridBagConstraints.gridy = 15;
        gridBagConstraints.anchor = java.awt.GridBagConstraints.SOUTHWEST;
        gridBagConstraints.insets = new java.awt.Insets(0, 8, 0, 8);
        controlPanel.add(randomColourButton, gridBagConstraints);

        tileCheckBox.setSelected(true);
        tileCheckBox.setText("Tile the Image to Cover the Entire Window");
        tileCheckBox.setToolTipText("Repeat the Texture to Cover the Entire Canvas");
        tileCheckBox.setOpaque(false);
        tileCheckBox.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                tileCheckBoxActionPerformed(evt);
            }
        });
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 16;
        gridBagConstraints.gridwidth = 3;
        gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
        gridBagConstraints.insets = new java.awt.Insets(8, 8, 0, 8);
        controlPanel.add(tileCheckBox, gridBagConstraints);

        presetsLabel.setText("Presets");
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 2;
        gridBagConstraints.gridy = 2;
        gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
        gridBagConstraints.insets = new java.awt.Insets(8, 0, 2, 8);
        controlPanel.add(presetsLabel, gridBagConstraints);

        presetDropDown.setFont(presetDropDown.getFont().deriveFont(presetDropDown.getFont().getSize()-1f));
        presetDropDown.setModel( new DefaultComboBoxModel( PRESET_NAMES ) );
        presetDropDown.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                presetDropDownActionPerformed(evt);
            }
        });
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 2;
        gridBagConstraints.gridy = 3;
        gridBagConstraints.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
        gridBagConstraints.insets = new java.awt.Insets(0, 4, 0, 8);
        controlPanel.add(presetDropDown, gridBagConstraints);

        randomizeCheck.setSelected(true);
        randomizeCheck.setText("Re-randomize System State at Start of Each Run");
        randomizeCheck.setOpaque(false);
        randomizeCheck.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                randomizeCheckActionPerformed(evt);
            }
        });
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 9;
        gridBagConstraints.gridwidth = 3;
        gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
        gridBagConstraints.insets = new java.awt.Insets(8, 8, 0, 8);
        controlPanel.add(randomizeCheck, gridBagConstraints);

        randomizeNowBtn.setFont(randomizeNowBtn.getFont().deriveFont(randomizeNowBtn.getFont().getSize()-2f));
        randomizeNowBtn.setText("Randomize System State Now");
        randomizeNowBtn.setEnabled(false);
        randomizeNowBtn.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                randomizeNowBtnActionPerformed(evt);
            }
        });
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 10;
        gridBagConstraints.gridwidth = 3;
        gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
        gridBagConstraints.insets = new java.awt.Insets(4, 24, 0, 8);
        controlPanel.add(randomizeNowBtn, gridBagConstraints);

        appearanceLabel.setFont(appearanceLabel.getFont().deriveFont(appearanceLabel.getFont().getStyle() | java.awt.Font.BOLD, appearanceLabel.getFont().getSize()+2));
        appearanceLabel.setText("Texture Appearance");
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 11;
        gridBagConstraints.gridwidth = 3;
        gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
        gridBagConstraints.insets = new java.awt.Insets(16, 8, 0, 8);
        controlPanel.add(appearanceLabel, gridBagConstraints);
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 12;
        gridBagConstraints.gridwidth = 3;
        gridBagConstraints.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints.weightx = 1.0;
        gridBagConstraints.insets = new java.awt.Insets(0, 8, 0, 8);
        controlPanel.add(appearanceSeparator, gridBagConstraints);

        advancedLabel.setFont(advancedLabel.getFont().deriveFont(advancedLabel.getFont().getStyle() | java.awt.Font.BOLD, advancedLabel.getFont().getSize()+2));
        advancedLabel.setText("More Options");
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 5;
        gridBagConstraints.gridwidth = 3;
        gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
        gridBagConstraints.insets = new java.awt.Insets(16, 8, 0, 8);
        controlPanel.add(advancedLabel, gridBagConstraints);
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 6;
        gridBagConstraints.gridwidth = 3;
        gridBagConstraints.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints.weightx = 1.0;
        gridBagConstraints.insets = new java.awt.Insets(0, 8, 0, 8);
        controlPanel.add(advancedSeparator, gridBagConstraints);

        basicLabel.setFont(basicLabel.getFont().deriveFont(basicLabel.getFont().getStyle() | java.awt.Font.BOLD, basicLabel.getFont().getSize()+2));
        basicLabel.setText("The Basics");
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 0;
        gridBagConstraints.gridwidth = 3;
        gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
        gridBagConstraints.insets = new java.awt.Insets(8, 8, 0, 8);
        controlPanel.add(basicLabel, gridBagConstraints);
        gridBagConstraints = new java.awt.GridBagConstraints();
        gridBagConstraints.gridx = 0;
        gridBagConstraints.gridy = 1;
        gridBagConstraints.gridwidth = 3;
        gridBagConstraints.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints.weightx = 1.0;
        gridBagConstraints.insets = new java.awt.Insets(0, 8, 0, 8);
        controlPanel.add(basicSeparator, gridBagConstraints);

        getContentPane().add(controlPanel, java.awt.BorderLayout.WEST);

        visualizer = new TuringSystemVisualizer( MAXIMUM_SYSTEM_SIZE, MAXIMUM_SYSTEM_SIZE );
        visualizerPanel.add( visualizer );
        visualizer.setTiled( tileCheckBox.isSelected() );
        imageSizeDropDown.setSelectedIndex( 2 );
        // copy the standard so we can swap between it and an "error" border
        textFieldBorder = constAField.getBorder();

        visualizerPanel.setBackground(java.awt.Color.white);
        visualizerPanel.setBorder(javax.swing.BorderFactory.createMatteBorder(1, 0, 1, 1, new java.awt.Color(0, 0, 0)));
        visualizerPanel.setLayout(new javax.swing.BoxLayout(visualizerPanel, javax.swing.BoxLayout.LINE_AXIS));
        getContentPane().add(visualizerPanel, java.awt.BorderLayout.CENTER);

        titlePanel.setBackground(java.awt.Color.black);
        titlePanel.setToolTipText( ABOUT_TIP );

        titleLabel.setFont(titleLabel.getFont().deriveFont(titleLabel.getFont().getStyle() | java.awt.Font.BOLD, titleLabel.getFont().getSize()+1));
        titleLabel.setForeground(java.awt.Color.white);
        titleLabel.setText( TITLE );
        titleLabel.setToolTipText( ABOUT_TIP );
        titlePanel.add(titleLabel);

        getContentPane().add(titlePanel, java.awt.BorderLayout.PAGE_START);
    }

    private void iterationsSliderStateChanged(javax.swing.event.ChangeEvent evt) {
		iterationsLabel.setText( "Number of Iterations: " + formatter.format( iterationsSlider.getValue() ) );
	}

    private void imageSizeDropDownActionPerformed(java.awt.event.ActionEvent evt) {
		int newBaseSize = baseSize;
		// parse the new size directly from the drop down item text
		Object o = imageSizeDropDown.getSelectedItem();
		String s = "";
		if( o != null ) {
			try {
				s = ((String) o);
				s = s.substring( s.lastIndexOf( ' ' ) + 1, s.length() - 1 );
				newBaseSize = Integer.parseInt( s );
			} catch( NumberFormatException e ) {
				throw new AssertionError( "Size drop down item has illegal " +
						"format---must end in \" n)\"" );
			}

			if( newBaseSize != baseSize ) {
				baseSize = newBaseSize;

				if( solver != null ) {
					solver.stop();
				}

				solver = new TuringSystemSolver( new TuringSystemSolver.ResultListener() {

					public void solverResultEvent( int iteration, double[][] result ) {
						visualizer.updateImage( result, baseSize, baseSize );
						final int i = iteration;
						SwingUtilities.invokeLater( new Runnable() {

							public void run() {
								progressBar.setValue( i );
								progressBar.setString( formatter.format( i ) );
							}
						} );
					}
				}, baseSize, baseSize );

				solver.randomize();
			}
		}
    }

	/** Generate a new texture. */
    private void okButtonActionPerformed(java.awt.event.ActionEvent evt) {
		iterations = iterationsSlider.getValue();
		progressBar.setMaximum( iterations );
		solver.stop();
		if( randomizeCheck.isSelected() ) {
			solver.randomize();
		}
		CA = parseConstant( constAField, CA );
		CB = parseConstant( constBField, CB );

		solver.solve( iterations, CA, CB );
	}

	private void colourButtonActionPerformed(java.awt.event.ActionEvent evt) {
		boolean isMin = evt.getSource() == minColourButton;

		Color min = minColourButton.getBackground();
		Color max = maxColourButton.getBackground();

		Component parent = isMin ? minColourButton : maxColourButton;

		Color pick = JColorChooser.showDialog( parent, "Choose Colour", isMin ? min : max );
		if( pick != null ) {
			if( isMin ) {
				min = pick;
			} else {
				max = pick;
			}
			visualizer.setColors( min, max );
			minColourButton.setBackground( min );
			maxColourButton.setBackground( max );
		}
	}

	private void randomColourButtonActionPerformed(java.awt.event.ActionEvent evt) {
		float hue = (float) Math.random();
		float sat = (float) Math.random();
		float bri = (float) Math.random() * 0.667f;
		Color min = new Color( Color.HSBtoRGB( hue, sat, bri ) );

		hue += (float) Math.random() * 26f / 180f;
		bri = 0.75f + (float) Math.random() / 4f;
		Color max = new Color( Color.HSBtoRGB( hue, sat, bri ) );

		if( Math.random() >= 0.5f ) {
			Color temp = min;
			min = max;
			max = temp;
		}

		minColourButton.setBackground( min );
		maxColourButton.setBackground( max );
		visualizer.setColors( min, max );
	}

	private void tileCheckBoxActionPerformed(java.awt.event.ActionEvent evt) {
		visualizer.setTiled( tileCheckBox.isSelected() );
	}

	private void presetDropDownActionPerformed(java.awt.event.ActionEvent evt) {
		int i = presetDropDown.getSelectedIndex();
		if( i >= 0 ) {
			constAField.setText( formatter.format( PRESET_CONSTANTS[i * 2] ) );
			constBField.setText( formatter.format( PRESET_CONSTANTS[i * 2 + 1] ) );
		}
	}

	private void randomizeCheckActionPerformed(java.awt.event.ActionEvent evt) {
		randomizeNowBtn.setEnabled( !randomizeCheck.isSelected() );
	}

	private void randomizeNowBtnActionPerformed(java.awt.event.ActionEvent evt) {
		solver.stop();
		solver.randomize();
	}


	/**
	 * Parse the content of <code>field</code> for a double value.
	 * If the content is a valid double, its value is returned.
	 * Otherwise, <code>defaultValue</code> is returned and the field
	 * is highlighted with a red border.
	 *
	 * @return the value in <code>field</code>, or <code>defaultValue</code>
	 */
	private double parseConstant( JTextField field, double defaultValue ) {
		double value;
		try {
			value = formatter.parse( field.getText() ).doubleValue();
			field.setBorder( textFieldBorder );
		} catch( ParseException e ) {
			field.setText( formatter.format( defaultValue ) );
			value = defaultValue;
			field.setBorder( errorBorder );
		}
		return value;
	}

    private javax.swing.JTextField constAField;
    private javax.swing.JTextField constBField;
    private javax.swing.JComboBox imageSizeDropDown;
    private javax.swing.JLabel iterationsLabel;
    private javax.swing.JSlider iterationsSlider;
    private javax.swing.JButton maxColourButton;
    private javax.swing.JButton minColourButton;
    private javax.swing.JButton okButton;
    private javax.swing.JComboBox presetDropDown;
    private javax.swing.JLabel presetsLabel;
    private javax.swing.JProgressBar progressBar;
    private javax.swing.JButton randomColourButton;
    private javax.swing.JCheckBox randomizeCheck;
    private javax.swing.JButton randomizeNowBtn;
    private javax.swing.JCheckBox tileCheckBox;
    private javax.swing.JLabel titleLabel;
    private javax.swing.JPanel titlePanel;

    private TuringSystemSolver solver;
    private int baseSize = 64;
	private int iterations = 2000;
    private double CA = 3.5d, CB = 16d;
    private TuringSystemVisualizer visualizer;
    private Border textFieldBorder;
	private Border errorBorder = BorderFactory.createLineBorder( Color.RED, 2 );
	private NumberFormat formatter = NumberFormat.getNumberInstance();

	private static final String TITLE = "Turing Reaction-Diffusion Morphogenesis";
	private static final String ABOUT_TIP = "<html><b>Christopher G. Jennings</b>" +
			" (Version 4)<br>http://cgjennings.ca";
	private static final String DRAG_TIP = "<br>&nbsp;<br>Drag This Title Bar" +
			" to Separate From Web Page";
}





/**
 * A component that displays a visualization of a {@link TuringSystemSolver}
 * result.
 */
class TuringSystemVisualizer extends JComponent {
	public static final Color DEFAULT_CMIN = new Color( 0x3c0009 );
	public static final Color DEFAULT_CMAX = new Color( 0xffff3a );

	/**
	 * Create a visualizer for a {@link TuringSystemSolver} with a maximum
	 * size of <code>width</code> x <code>height</code>.
	 *
	 * @param width the maximum width of the systems to be visualized
	 * @param height the maximum height of the systems to be visualized
	 */
    TuringSystemVisualizer( int width, int height ) {
		super();
		setSize( width, height );
		colours = new int[256];
		setColors( DEFAULT_CMIN, DEFAULT_CMAX );
    }

    /**
	 * Set the colours to use for visualizing results. The image colours will
	 * vary smoothly from <code>cmin</code> (representing the minimum value in
	 * the system) to <code>cmax</code> (representing the maximum value).
	 *
	 * @param cmin colour to use for the minimum value
	 * @param cmax colour to use for the maximum value
	 */
    public synchronized void setColors( Color cmin, Color cmax ) {
		int Rlo = cmin.getRed();
		int Glo = cmin.getGreen();
		int Blo = cmin.getBlue();
		int Rhi = cmax.getRed();
		int Ghi = cmax.getGreen();
		int Bhi = cmax.getBlue();

		for( int i=0; i<256; ++i ) {
			colours[i] = scale( i, Rlo, Rhi ) << 16 |
				scale( i, Glo, Ghi ) << 8  |
				scale( i, Blo, Bhi );
		}
		updateImage();
    }

    /**
	 * Calculate an interpolated colour component that is i/255 of the way between lo and hi.
	 */
    private static int scale( int i, int lo, int hi ) {
		return lo + (hi-lo) * i / 255;
    }


	/**
	 * Return the colours currently used to visualize results. The colours are
	 * returned in an array in <code>cmin</code>, <code>cmax</code> order
	 * (see {@link #setColors}).
	 *
	 * @return an array of the colours used for the extreme values in the system
	 */
	public Color[] getColors() {
		return new Color[] { new Color( colours[0] ), new Color( colours[255] ) };
	}

	/**
	 * Set the tiling mode of the component. If tiling is enabled, the component
	 * will create a tiled image by repeating the visualization until the
	 * entire component is filled. Otherwise, the tile is centered in the
	 * middle of the component.
	 * @param tile whether or not to enable tiling
	 */
	public void setTiled( boolean tile ) {
		this.tile = tile;
		repaint();
	}

	/**
	 * Return <code>true</code> if the component will tile the image.
	 * @return <code>true</code> if tiling is enabled
	 */
	public boolean isTiled() {
		return tile;
	}

    /**
	 * Update the visualiztion with a new data set, causing the display to be redrawn.
	 */
    public void updateImage( double data[][], int width, int height ) {
		if( image==null || image.getWidth() != width || image.getHeight() != height ) {
			image = new BufferedImage( width, height, BufferedImage.TYPE_INT_RGB );
			scaledData = new int[width][height];
		}

		int i, j;

		// find range of concentrations
		double high = Double.NEGATIVE_INFINITY;
		double low = Double.POSITIVE_INFINITY;
		for( i=0; i<height; ++i ) {
			for( j=0; j<width; ++j ) {
				double val = data[i][j];
				if( val > high ) high = val;
				else if( val < low  ) low  = val;
			}
		}

		// scale data to lie within 0--255
		for( i=0; i<height; ++i ) {
			for( j=0; j<width; ++j ) {
				int scaled = (int) ((data[i][j] - low) * 255.0 / (high-low));
				scaled = Math.max( 0, Math.min( 255, scaled ) );
				scaledData[i][j] = scaled;
			}
		}

		updateImage();
    }

	/**
	 * Redraw the image using the current scaled data and colour set.
	 */
	private synchronized void updateImage() {
		if( image == null ) return;

		int width = image.getWidth();
		int height = image.getHeight();

		// update the image using the scaled data
		for( int y=0; y<height; ++y ) {
			for( int x=0; x<width; ++x ) {
				image.setRGB( x, y, colours[ scaledData[x][y] ] );
			}
		}

		// this is thread-safe
		repaint();
	}

	@Override
    public void paintComponent( Graphics g ) {
		super.paintComponent( g );
		if( image == null  ) return;

		if( tile ) {
			for( int y=0; y < getHeight(); y += image.getHeight() ) {
				for( int x=0; x < getWidth(); x += image.getWidth() ) {
					g.drawImage( image, x, y, null );
				}
			}
		} else {
			g.drawImage( image,
					(getWidth() - image.getWidth())/2,
					(getHeight() - image.getHeight())/2,
					null );
		}
    }

    private BufferedImage image;
	private int[][] scaledData;
    private int[] colours;
	private volatile boolean tile = false;
}





/**
 * Solve Turing Reaction-Diffusion equations using Euler's method.
 * A {@link TuringSystemSolver.ResultListener} will be
 * notified of interim and final results.
 */
class TuringSystemSolver  {
	/**
	 * A listener interface for events generated by a {@link TuringSystemSolver}.
	 */
	interface ResultListener {
		/**
		 * An event generated by a {@link TuringSystemSolver} whenever a new result
		 * is available. The value of <code>iteration</code> may be used to determine if
		 * it is an initial, final, or interim result. The current state of the
		 * system is passed in <code>result</code>, which is an array of the
		 * same size as the requested width and height of the system.
		 * This array should be considered read-only.
		 *
		 * @param iteration the iteration number represented by this result,
		 *                  from 0 to the total iterations requested
		 * @param result an array representing the current system state
		 */
		void solverResultEvent( int iteration, double[][] result );
	}

    private int iterations;
    private double CA, CB;
	private volatile Thread solveThread;

    public TuringSystemSolver( ResultListener listener, int width, int height ) {
		this.listener = listener;
		Ao = new double[width][height];
		An = new double[width][height];
		Bo = new double[width][height];
		Bn = new double[width][height];
		this.width = width;
		this.height = height;
    }

    public void solve( int iterations, double CA, double CB ) {
		if( Thread.currentThread() == solveThread )
			throw new AssertionError( "called solve() from solveThread" );

		stop();
		this.iterations = iterations;
		this.CA = CA;
		this.CB = CB;
		solveThread = new Thread( new Runnable() {
			public void run() {
				solveImpl();
			}
		} );
		solveThread.start();
    }

    private void solveImpl() {
		int n, i, j, iplus1, iminus1, jplus1, jminus1;
		double DiA, ReA, DiB, ReB;

		long lastUpdateTime = System.nanoTime(), currentTime;
		int  lastUpdateFrame = 0;

		for( n=0; n<iterations; ++n ) {
			for( i=0; i<height; ++i) {
				// treat the surface as a torus by wrapping at the edges
				iplus1 = i+1;
				iminus1 = i-1;
				if( i == 0 ) iminus1 = height - 1;
				if( i == height - 1 ) iplus1 = 0;

				for( j=0; j<width; ++j ) {
					jplus1 = j+1;
					jminus1 = j-1;
					if( j == 0 ) jminus1 = width - 1;
					if( j == width - 1 ) jplus1 = 0;

					// Component A
					DiA = CA * ( Ao[iplus1][j] - 2.0 * Ao[i][j] + Ao[iminus1][j]
						   + Ao[i][jplus1] - 2.0 * Ao[i][j] + Ao[i][jminus1] );
					ReA = Ao[i][j] * Bo[i][j] - Ao[i][j] - 12.0;
					An[i][j] = Ao[i][j] + 0.01 * (ReA + DiA);
					if( An[i][j] < 0.0 ) An[i][j] = 0.0;

					// Component B
					DiB = CB * ( Bo[iplus1][j] - 2.0 * Bo[i][j] + Bo[iminus1][j]
						   + Bo[i][jplus1] - 2.0 * Bo[i][j] + Bo[i][jminus1] );
					ReB = 16.0 - Ao[i][j] * Bo[i][j];
					Bn[i][j] = Bo[i][j] + 0.01 * (ReB + DiB);
					if( Bn[i][j] < 0.0 ) Bn[i][j]=0.0;
				}
				if( Thread.interrupted() ) return;
			}

			// The visualizer will be updated whenever UPDATE_RATE_FRAMES iterations
			// have been calculated, or when the time since the last update
			// has been at least UPDATE_RATE_TIME nanoseconds (whichever happens
			// first).
			currentTime = System.nanoTime();
			if( ((currentTime - lastUpdateTime) >= UPDATE_RATE_TIME)
			    || ((n - lastUpdateFrame) >= UPDATE_RATE_FRAMES) ) {
				lastUpdateTime = currentTime;
				lastUpdateFrame = n;
				sendFrameToListener( n, An );
			}
			// Swap Ao for An, Bo for Bn for next frame
			swapBuffers();
		}
		// send the final data set to be displayed
		sendFrameToListener( n, An );
		solveThread = null;
    }

    /**
	 * Stop an in-progress solution, if any.
	 */
    public synchronized void stop() {
		if( Thread.currentThread() == solveThread )
			throw new AssertionError( "called stop() from solveThread" );

		Thread thread = solveThread;
		if( thread != null ) {
			solveThread = null;
			thread.interrupt();

			boolean killed = false;
			while( !killed ) {
				try { thread.join(); killed = true; }
				catch( InterruptedException e ) {}
			}
		}
    }

    /**
	 * Set the system to an initial noise state.
	 * This should normally called before solving.
	 */
    public void randomize() {
		stop();
		for( int i=0; i<height; ++i ) {
			for( int j=0; j<width; ++j ) {
				Ao[i][j] = rand.nextDouble() * 12.0 + rand.nextGaussian() * 2.0;
				Bo[i][j] = rand.nextDouble() * 12.0 + rand.nextGaussian() * 2.0;
			}
		}
		sendFrameToListener( 0, Ao );
    }

	private void swapBuffers() {
		double[][] temp = Ao;
		Ao = An;
		An = temp;
		temp = Bo;
		Bo = Bn;
		Bn = temp;
	}

    /**
	 * Sends the current system state to the listener.
	 */
    protected void sendFrameToListener( int iteration, double[][] data ) {
		// for efficiency and simplicity, we do not send a copy of A;
		// that means that the listener can modify our data
		// if this is a concern, clone the array before sending
		listener.solverResultEvent( iteration, data );
    }

    private ResultListener listener;
	// System state at the old (Ao, Bo) and new (An, Bn) time steps.
    private double[][] Ao, Bo, An, Bn;
    private int width, height;
    private Random rand = new Random();

    /** Maximum number of iterations between update frames. */
    protected static final int UPDATE_RATE_FRAMES = 20;
    /** Maximum number of nanoseconds between update frames. */
    protected static final long UPDATE_RATE_TIME = 200000000L;
}

