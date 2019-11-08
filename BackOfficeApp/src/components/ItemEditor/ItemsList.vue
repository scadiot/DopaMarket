<template>
  <div>
    <div v-for="item in items" :key="item.id" v-on:click="itemClicked(item)">{{ item.name }}</div>
  </div>
</template>

<script>
export default {
    props: ['items'],
    methods: {
        itemClicked: function(item) {
            this.$emit('itemClicked', item)
        }
    }
}
</script>

<style>

</style>