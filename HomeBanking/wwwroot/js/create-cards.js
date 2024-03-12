var app = new Vue({
    el:"#app",
    data:{
        errorToats: null,
        errorMsg: null,
        cardType:"none",
        cardColor:"none",
    },
    methods:{
        formatDate: function(date){
            return new Date(date).toLocaleDateString('en-gb');
        },
        signOut: function(){
            sessionStorage.clear();
            window.location.href = "/index.html";
        },
        create: function(event){
            event.preventDefault();
            if(this.cardType == "none" || this.cardColor == "none"){
                this.errorMsg = "You must select a card type and color";  
                this.errorToats.show();
            }else{
/*                 let config = {
                    headers: {
                        'content-type': 'application/x-www-form-urlencoded'
                    }
                }
                axios.post(`http://localhost:8080/api/clients/current/cards?cardType=${this.cardType}&cardColor=${this.cardColor}`,config)
                .then(response => window.location.href = "/web/cards.html")
                .catch((error) =>{
                    this.errorMsg = error.response.data;  
                    this.errorToats.show();
                }) */
                let token = sessionStorage.getItem('TOKEN');
                axios.post('/api/clients/current/cards',{
                    type: this.cardType,
                    color: this.cardColor,
                },
                {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
                }
                )
                    .then(() => { window.location.href = "/cards.html" })
                    .catch(() => {
                        this.errorMsg = "Sign up failed, check the information"
                        this.errorToats.show();
                    })
            }
        }
    },
    mounted: function(){
        this.errorToats = new bootstrap.Toast(document.getElementById('danger-toast'));
    }
})