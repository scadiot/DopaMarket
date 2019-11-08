<template>
  <div>
    <ItemsList :items="items" v-on:itemClicked="itemClicked" />
  </div>
</template>

<script>
import axios from 'axios';
import ItemsList from './ItemsList.vue'

export default {
  components: {
    ItemsList
  },
  data: function () {
      return {
          items: [ ]
      }
  },
  created() {
      axios.get('http://localhost:60037/Api/Item/GetAllItems')
      .then(response => {
          // handle success
          this.items = response.data;
      })
      .catch(function (error) {
          // handle error
          console.log(error);
      })
      .finally(function () {
          // always executed
          return;
      });
    },
    methods: {
        itemClicked: function(item) {
          console.log(item.name);
            //var targetId = event.currentTarget.id;
            //console.log(targetId); // returns 'foo'
            //this.$emit('myEvent', event.target.item.id)
        }
    }
}
</script>

<style>

</style>