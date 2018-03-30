
public class ListInvert {
	public int data;
	public ListInvert next;
	
	public ListInvert( int data ){
		this.data = data;
		this.next = null;
	}
	
	public ListInvert[] locate( ListInvert head, int i ){
		ListInvert[] aux = new ListInvert[]{null, head};
		for( int k=1; k<i; k++ ){
			aux[0] = aux[1];
			if( aux[1] != null ){
				aux[1] = aux[1].next;
			}else{
				aux[1] = null;
			}
		}
		return aux;
	}
	
	public void invert( ListInvert head, int i, int j ){
		ListInvert[] start = locate(head, i);
		ListInvert[] end = locate(head, j);
		ListInvert aux = start[1];
		while( aux.next != end[1] ){
			ListInvert tmp1 = aux.next;
			ListInvert tmp2 = aux.next.next;
			aux.next.next = aux;
		}
	}
	
	public static void main( String[] args ){
		
	}
}
