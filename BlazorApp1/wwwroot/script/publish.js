require('require');
let { clean, restore, build, pack, push, publish } = require('gulp-dotnet-cli');
const path = require('path');
let gulp = require('gulp');

var shelljs = global.shelljs = global.shelljs || require('shelljs');
var fs = global.fs = global.fs || require('fs');

function projectCompilation(){
    var runSequence = require('run-sequence');
    runSequence('clean', 'styles', 'bundle', 'bundle-showcase', done);
}

gulp.task('clean', () => {
    return gulp.src(['./EJSBlazorEditor/EJSBlazorEditor.csproj'], { read: false })
        .pipe(clean());
});

gulp.task('restore', () => {
    return gulp.src(['./EJSBlazorEditor/EJSBlazorEditor.csproj'], { read: false })
        .pipe(restore({ echo: true }));
});

gulp.task('publish', ()=>{
    return gulp.src('./EJSBlazorEditor/EJSBlazorEditor.csproj', {read: false})
                .pipe(publish({configuration: 'Release', output: './Publish'}));
});
