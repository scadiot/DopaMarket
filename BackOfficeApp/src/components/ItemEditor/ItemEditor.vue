<template>
  <div>
    <input v-model="item.name" >
  </div>
</template>

<script>
import axios from 'axios';
import ItemsList from './ItemsList.vue'

export default {
    props: ['item']
}
</script>

<style>

</style>